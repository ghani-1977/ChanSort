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
      this.rawImageCollection.Images.SetKeyName(0, "0000.png");
      this.rawImageCollection.Images.SetKeyName(1, "0001.png");
      this.rawImageCollection.Images.SetKeyName(2, "0002.png");
      this.rawImageCollection.Images.SetKeyName(3, "0003.png");
      this.rawImageCollection.Images.SetKeyName(4, "0004.png");
      this.rawImageCollection.Images.SetKeyName(5, "0005.png");
      this.rawImageCollection.Images.SetKeyName(6, "0006.png");
      this.rawImageCollection.Images.SetKeyName(7, "0007.png");
      this.rawImageCollection.Images.SetKeyName(8, "0008.png");
      this.rawImageCollection.Images.SetKeyName(9, "0009.png");
      this.rawImageCollection.Images.SetKeyName(10, "0010.png");
      this.rawImageCollection.Images.SetKeyName(11, "0011.png");
      this.rawImageCollection.Images.SetKeyName(12, "0012.png");
      this.rawImageCollection.Images.SetKeyName(13, "0013.png");
      this.rawImageCollection.Images.SetKeyName(14, "0014.png");
      this.rawImageCollection.Images.SetKeyName(15, "0015.png");
      this.rawImageCollection.Images.SetKeyName(16, "0016.png");
      this.rawImageCollection.Images.SetKeyName(17, "0017.png");
      this.rawImageCollection.Images.SetKeyName(18, "0018.png");
      this.rawImageCollection.Images.SetKeyName(19, "0019.png");
      this.rawImageCollection.Images.SetKeyName(20, "0020.png");
      this.rawImageCollection.Images.SetKeyName(21, "0021.png");
      this.rawImageCollection.Images.SetKeyName(22, "0022.png");
      this.rawImageCollection.Images.SetKeyName(23, "0023.png");
      this.rawImageCollection.Images.SetKeyName(24, "0024.png");
      this.rawImageCollection.Images.SetKeyName(25, "0025.png");
      this.rawImageCollection.Images.SetKeyName(26, "0026.png");
      this.rawImageCollection.Images.SetKeyName(27, "0027.png");
      this.rawImageCollection.Images.SetKeyName(28, "0028.png");
      this.rawImageCollection.Images.SetKeyName(29, "0029.png");
      this.rawImageCollection.Images.SetKeyName(30, "0030.png");
      this.rawImageCollection.Images.SetKeyName(31, "0031.png");
      this.rawImageCollection.Images.SetKeyName(32, "0032.png");
      this.rawImageCollection.Images.SetKeyName(33, "0033.png");
      this.rawImageCollection.Images.SetKeyName(34, "0034.png");
      this.rawImageCollection.Images.SetKeyName(35, "0035.png");
      this.rawImageCollection.Images.SetKeyName(36, "0036.png");
      this.rawImageCollection.Images.SetKeyName(37, "0037.png");
      this.rawImageCollection.Images.SetKeyName(38, "0038.png");
      this.rawImageCollection.Images.SetKeyName(39, "0039.png");
      this.rawImageCollection.Images.SetKeyName(40, "0040.png");
      this.rawImageCollection.Images.SetKeyName(41, "0041.png");
      this.rawImageCollection.Images.SetKeyName(42, "0042.png");
      this.rawImageCollection.Images.SetKeyName(43, "0043.png");
      this.rawImageCollection.Images.SetKeyName(44, "0044.png");
      this.rawImageCollection.Images.SetKeyName(45, "0045.png");
      this.rawImageCollection.Images.SetKeyName(46, "0046.png");
      this.rawImageCollection.Images.SetKeyName(47, "0047.png");
      this.rawImageCollection.Images.SetKeyName(48, "0048.png");
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
