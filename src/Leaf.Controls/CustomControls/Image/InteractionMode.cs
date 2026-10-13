namespace Leaf.Controls.CustomControls
{
    public enum InteractionMode
    {
        /// <summary>
        /// 图像平移模式
        /// </summary>
        Pan,

        /// <summary>
        /// 绘制ROI模式
        /// </summary>
        DrawROI,

        /// <summary>
        /// 编辑ROI模式
        /// </summary>
        Edit,

        /// <summary>
        /// 选择ROI模式
        /// </summary>
        Select,

        /// <summary>
        /// 取色模式：左键点击图像取该点像素，通过 PixelPickedCommand 回传原图坐标
        /// </summary>
        PickColor
    }
}
