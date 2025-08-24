namespace Clairvoyance
{
    public partial class Displayer : Form
    {
        private UiOperator ui;
        private WinOperator win;
        private Dictionary<string, string> nameSizePairs = [];

        public Displayer(string root, WinOperator win)
        {
            InitializeComponent();

            this.win = win;
            this.ui = new(win, root);

            SetNameSizePairs(root);

            AddLabels();

            ui.WatchNameChange(() => this.Invoke(() => this.Close()));
            ui.WatchListChange(() => this.Invoke(() =>
            {
                this.Controls.Clear();
                AddLabels();
            }));
        }

        private void SetNameSizePairs(string root)
        {
            var dirs = Directory.GetDirectories(root);
            if (dirs.Length == 0)
            {
                throw new Exception();
            }
            foreach (var dir in dirs)
            {
                nameSizePairs.TryAdd(dir, FolderSizeCalculator.FormatSize(FolderSizeCalculator.GetDirectorySize(dir)));
            }
        }

        private void AddLabels()
        {
            Rect rect = ui.GetWindowPosition();
            win.FollowParent(this.Handle, rect);
            this.Width = (int)rect.Width;
            this.Height = (int)rect.Height;
            this.StartPosition = FormStartPosition.Manual;

            foreach (var (name, size) in nameSizePairs)
            {
                int row;
                try
                {
                    row = (int)(ui.GetDirRow(Path.GetFileName(name)).Y - rect.Y);
                }
                catch
                {
                    continue;
                }

                Label lbl = new()
                {
                    Text = size,
                    AutoSize = false,
                    Font = new Font("Yu Gothic UI", 9, FontStyle.Bold),
                    Location = new Point(0, row),
                    TextAlign = ContentAlignment.MiddleRight
                };
                this.Controls.Add(lbl);
            }
        }

        private void Displayer_Load(object sender, EventArgs e)
        {

        }

    }
}
