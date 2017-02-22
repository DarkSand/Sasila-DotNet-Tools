using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;

namespace Sasila.Common.Tools
{
    public static class ImageSplit
    {
        /// <summary>
        /// 分割图片
        /// </summary>
        /// <param name="img">原始图片</param>
        /// <param name="portionWidth">分割后图片宽</param>
        /// <param name="portionHeight">分割后图片高</param>
        /// <returns>返回值</returns>
        public static Image[] Split(Image img, int portionWidth, int portionHeight)
        {
            int imgIndex = 0;
            //原始图片能分割成多少块新规格的图片
            int maxHeight = img.Height;
            int maxWidth = img.Width;
            int heightNumber = (maxHeight % portionHeight) > 0 ? (maxHeight / portionHeight) + 1 : (maxHeight / portionHeight);
            int widthNumber = (maxWidth % portionWidth) > 0 ? (maxWidth / portionWidth) + 1 : (maxWidth / portionWidth);
            int allNumber = heightNumber * widthNumber;

            Image[] images = new Image[allNumber];
            using (Bitmap imgSplitted = new Bitmap(portionWidth, portionHeight))
            {
                using (Graphics g = Graphics.FromImage(imgSplitted))
                {
                    for (int y = 0; y < maxHeight; y += portionHeight)
                    {
                        for (int x = 0; x < maxWidth; x += portionWidth)
                        {
                            try
                            {
                                g.Clear(Color.Transparent);
                                g.DrawImage(img, new Rectangle(0, 0, portionWidth, portionHeight), x, y, portionWidth, portionHeight, GraphicsUnit.Pixel);
                                g.Save();
                                images[imgIndex] = imgSplitted.Clone() as Image;
                                imgIndex++;
                            }
                            catch (Exception ex)
                            {
                                return images;
                            }
                        }
                    }
                }
            }
            return images;
        }
    }
}