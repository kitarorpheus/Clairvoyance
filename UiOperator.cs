using System.Windows.Automation;

namespace Clairvoyance
{
    public struct Rect
    {
        public double X, Y, Width, Height;

        public Rect(double x, double y, double width, double height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        public override string ToString() => $"X={X}, Y={Y}, W={Width}, H={Height}";
    }

    /// <summary>
    /// Wrapping UiAutomation.
    /// </summary>
    public class UiOperator
    {
        private WinOperator win;
        private string expectedFolderName;
        private System.Timers.Timer eventTimer;
        private Action onFolderChanged;

        public UiOperator(WinOperator win, string folderName)
        {
            this.win = win;
            this.expectedFolderName = folderName;
        }

        public void WatchNameChange(Action onChanged)
        {
            this.onFolderChanged = onChanged;
            IntPtr hWnd = win.GetHWnd();

            Automation.AddAutomationPropertyChangedEventHandler(
                AutomationElement.FromHandle(hWnd),
                TreeScope.Element,
                (sender, e) =>
                {
                    if (e.Property == AutomationElement.NameProperty)
                    {
                        string currentName = ((AutomationElement)sender).Current.Name;
                        if (!currentName.Contains(expectedFolderName))
                        {
                            onFolderChanged?.Invoke();
                        }
                    }
                },
                AutomationElement.NameProperty
            );
        }

        public void WatchListChange(Action onListChange)
        {
            AutomationElement list = AutomationElement.FromHandle(win.GetHWnd()).FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List)
            );

            if (list == null)
            {
                MessageBox.Show("ListView が見つかりません。");
                return;
            }

            // StructureChangedEvent を購読
            Automation.AddStructureChangedEventHandler(
                list,
                TreeScope.Subtree,
                (sender, e) =>
                {
                    eventTimer?.Stop();
                    eventTimer?.Dispose();

                    eventTimer = new(75)
                    {
                        AutoReset = false
                    };

                    eventTimer.Elapsed += (s, ev) =>
                    {
                        onListChange?.Invoke();
                        eventTimer.Dispose();
                    };
                    
                    eventTimer.Start();
                    //MessageBox.Show($"[EVENT] {e.StructureChangeType} が発生");
                }
            );
        }

        private static Rect GetElementBounds(AutomationElement element)
        {
            if (element == null)
                throw new Exception();

            object rectObj = element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

            if (rectObj != null)
            {
                 string[] parts = rectObj.ToString().Split(',');

                if (parts.Length == 4 &&
                    double.TryParse(parts[0], out double x) &&
                    double.TryParse(parts[1], out double y) &&
                    double.TryParse(parts[2], out double width) &&
                    double.TryParse(parts[3], out double height))
                {
                    return new Rect(x, y, width, height);
                }
            }
            throw new Exception();
        }

        private AutomationElementCollection GetSizeColumn()
        {
            IntPtr hWnd = win.GetHWnd();

            return AutomationElement.FromHandle(hWnd).FindAll(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "System.Size")
                )
            );
        }

        public Rect GetWindowPosition()
        {
            var sizeColumn = GetSizeColumn();

            if (sizeColumn.Count > 0)
            {
                var first = GetElementBounds(sizeColumn[0]);
                var last = GetElementBounds(sizeColumn[sizeColumn.Count - 1]);
                return new Rect(first.X, first.Y, first.Width, last.Y + last.Height - first.Y);
            }
            else
            {
                throw new Exception();
            }
        }
        
        public Rect GetDirRow(string elementName)
        {
            IntPtr hWnd = win.GetHWnd();

            var element = AutomationElement.FromHandle(hWnd).FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.NameProperty, elementName),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem)
                )
            );

            return GetElementBounds(element);
        }

        public int GetDif()
        {
            var sizeColumn = GetSizeColumn();

            if (sizeColumn.Count > 1)
            {
                var first = GetElementBounds(sizeColumn[0]);
                var last = GetElementBounds(sizeColumn[sizeColumn.Count - 1]);
                return (int)Math.Round((last.Y + last.Height - first.Y - first.Height) / (sizeColumn.Count - 1));
            }
            else
            {
                return 0;
            }
        }
    }
}
