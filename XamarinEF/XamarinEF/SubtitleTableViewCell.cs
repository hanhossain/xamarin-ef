using Foundation;
using UIKit;

namespace XamarinEF
{
    public class SubtitleTableViewCell : UITableViewCell
    {
        [Export("initWithStyle:reuseIdentifier:")]
        public SubtitleTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
            : base(UITableViewCellStyle.Subtitle, reuseIdentifier)
        {
        }
    }
}
