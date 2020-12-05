﻿using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using DevExpress.Utils;

namespace ChanSort.Ui
{
  #region class GlobalImageCollectionHolder
  /// <summary>
  /// This class must be a "Component" so we can use the Visual Studio Component Designer to modify the image collections
  /// that are used globally throughout the solution
  /// </summary>
  [ToolboxItem(false)]
  public class GlobalImageCollectionHolder : Component
  {
    private IContainer components;
    private ImageCollection rawImageCollection;
    private ImageCollection scaledImageCollection;

    #region ctor

    [Obsolete("Wrong constructor call generated by Forms Designer. Please restart Visual Studio and add 'this.components' as parameter")]
    public GlobalImageCollectionHolder() : this(null)
    {
    }

    public GlobalImageCollectionHolder(IContainer container)
    {
      InitializeComponent();
      this.scaledImageCollection = this.rawImageCollection;
      SetSharedImageCollectionImages(this.rawImageCollection);
      container?.Add(this);
    }
    #endregion

    #region Component Designer generated code

    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalImageCollectionHolder));
      this.rawImageCollection = new DevExpress.Utils.ImageCollection(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.rawImageCollection)).BeginInit();
      // 
      // rawImageCollection
      // 
      this.rawImageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("rawImageCollection.ImageStream")));
      this.rawImageCollection.Images.SetKeyName(47, "0047.png");
      ((System.ComponentModel.ISupportInitialize)(this.rawImageCollection)).EndInit();

    }

    #endregion

    #region SetSharedImageCollectionImages()
    private void SetSharedImageCollectionImages(ImageCollection imageCollection)
    {
      var fi = typeof(SharedImageCollection).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
      fi?.SetValue(null, imageCollection);
    }
    #endregion

    internal ImageCollection ImageCollection => scaledImageCollection;

    #region Scale()

    private float currentScaleFactor = 1;

    internal void Scale(float factor, bool relative)
    {
      var absFactor = relative ? this.currentScaleFactor * factor : factor;

      if (Math.Abs(absFactor - this.currentScaleFactor) < 0.01f)
        return;

      this.Scale(ref absFactor, this.rawImageCollection, ref this.scaledImageCollection);

      SetSharedImageCollectionImages(this.scaledImageCollection);

      this.currentScaleFactor = absFactor;
    }

    private void Scale(ref float absFactor, ImageCollection raw, ref ImageCollection scaled)
    {
      scaled?.Dispose();

      if (Math.Abs(absFactor - 1) < 0.01f)
      {
        scaled = raw;
        this.currentScaleFactor = 1;
        return;
      }

      var rawSize = raw.ImageSize;
      var newSize = new Size((int)(rawSize.Width * absFactor), (int)(rawSize.Height * absFactor));

      scaled = new ImageCollection();
      scaled.ImageSize = newSize;
      foreach (Image img in raw.Images)
        scaled.AddImage(ScaleImage(img, absFactor));
    }
    #endregion

    #region ScaleImage()

    private static Image ScaleImage(Image image, float absScaleFactor)
    {
      if (Math.Abs(absScaleFactor - 1) < 0.01)
        return image;

      int width = (int)(image.Width * absScaleFactor);
      int height = (int)(image.Height * absScaleFactor);
      var destRect = new Rectangle(0, 0, width, height);
      var destImage = new Bitmap(width, height);

      destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

      using var graphics = Graphics.FromImage(destImage);
      graphics.CompositingMode = CompositingMode.SourceCopy;
      graphics.CompositingQuality = CompositingQuality.HighQuality;
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      graphics.SmoothingMode = SmoothingMode.HighQuality;
      graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

      using var wrapMode = new ImageAttributes();
      wrapMode.SetWrapMode(WrapMode.TileFlipXY);
      graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

      return destImage;
    }
    #endregion

  }
  #endregion

  #region class GlobalImageCollection
  /// <summary>
  /// Subclass of SharedImageCollection with a CodeDomSerializer that suppresses problematic code in "Forms Designer generated code" blocks
  /// </summary>
  [DesignerSerializer(typeof(GlobalImageCollectionCodeDomSerializer), typeof(CodeDomSerializer))]
  [DesignerCategory("Code")] // do not ever open this class in a Component/Forms designer
  public partial class GlobalImageCollection : SharedImageCollection
  {
    // The static GlobalImageCollectionHolder instantiated here forces that it's internal SharedImageCollection is loaded
    // first (from this central assembly) before the (base) constructor of a GlobalImageCollection tries to load the images from the wrong assembly
    internal static readonly GlobalImageCollectionHolder Holder = new GlobalImageCollectionHolder(null);

    #region ctor

    [Obsolete("Wrong constructor call generated by Forms Designer. Restart Visual Studio and pass 'this.components' as parameter.")]
    public GlobalImageCollection() { }
    public GlobalImageCollection(IContainer container) : base(container) { }

    #endregion

    #region hacks to the SharedImageCollection: ImageSource, CreateInternalCollection()

    // Suppress code generation: instances of this class should not read/write any resources
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ImageCollection ImageSource => base.ImageSource;

    protected override ImageCollection CreateInternalCollection()
    {
      return Holder.ImageCollection;
    }
    #endregion

    // convenience members
    public static Images Images => Holder.ImageCollection.Images;

    public static void Scale(float factor, bool relative) => Holder.Scale(factor, relative);

  }
  #endregion

  #region class GlobalImageCollectionCodeDomSerializer

  internal class GlobalImageCollectionCodeDomSerializer : CodeDomSerializer
  {
    public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
    {
      var baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(SharedImageCollection), typeof(CodeDomSerializer));
      return baseSerializer.Deserialize(manager, codeObject);
    }

    public override object Serialize(IDesignerSerializationManager manager, object value)
    {
      var baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(SharedImageCollection), typeof(CodeDomSerializer));
      object codeObject = baseSerializer.Serialize(manager, value);

      // remove all generated code except for the member initialization
      CodeStatementCollection coll = codeObject as CodeStatementCollection;
      if (coll != null)
      {
        for (int i = coll.Count - 1; i >= 0; i--)
        {
          CodeStatement ex = coll[i];
          var assignment = ex as CodeAssignStatement;
          if (assignment == null || !(assignment.Left is CodeFieldReferenceExpression))
            coll.RemoveAt(i);
        }
      }
      return codeObject;
    }
  }
  #endregion
}
