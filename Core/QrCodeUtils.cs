using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace SS.Payment.Core
{
    public class QrCodeUtils
    {
        private QrCodeUtils()
        {
        }

        //输出二维码图片
        public static byte[] GetBuffer(string str)
        {
            var qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M,
                QRCodeVersion = 0,
                QRCodeScale = 4
            };

            //将字符串生成二维码图片
            var image = qrCodeEncoder.Encode(str, Encoding.Default);

            //保存为PNG到内存流  
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            return ms.GetBuffer();
        }
    }
}