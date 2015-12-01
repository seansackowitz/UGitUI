using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using LibGit2Sharp;
using System.Windows;

namespace UGitUI
{
    public class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        static Pen pen;

        static SolidColorBrush removedBackground;
        static SolidColorBrush addedBackground;
        static SolidColorBrush headerBackground;

        static DiffLineBackgroundRenderer()
        {
            removedBackground = new SolidColorBrush(Color.FromArgb(0x50, 0xd0, 0x54, 0x54)); removedBackground.Freeze();
            addedBackground = new SolidColorBrush(Color.FromArgb(0x50, 0x54, 0xd0, 0x94)); addedBackground.Freeze();
            headerBackground = new SolidColorBrush(Color.FromArgb(0xb0, 0xd0, 0xd0, 0xff)); headerBackground.Freeze();

            var blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)); blackBrush.Freeze();
            pen = new Pen(blackBrush, 0.0);
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView.Document.Text == "") return;

            foreach (var v in textView.VisualLines)
            {
                string diffLine = textView.Document.GetText(v.FirstDocumentLine);
                if (diffLine == "") continue;

                Brush brush = default(Brush);
                switch (GetLineStyle(diffLine))
                {
                    case DiffLineStyle.Header:
                        brush = headerBackground;
                        break;
                    case DiffLineStyle.Added:
                        brush = addedBackground;
                        break;
                    case DiffLineStyle.Deleted:
                        brush = removedBackground;
                        break;
                }

                Rect rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000).First();
                drawingContext.DrawRectangle(brush, pen, new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
            }
        }

        DiffLineStyle GetLineStyle(string line)
        {
            switch (line[0])
            {
                case '@':
                    return DiffLineStyle.Header;
                case '+':
                    return DiffLineStyle.Added;
                case '-':
                    return DiffLineStyle.Deleted;
                default:
                    return DiffLineStyle.Context;
            }
        }

        enum DiffLineStyle
        {
            Header,
            Added,
            Deleted,
            Context
        }
    }
}
