using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace banluu
{
    public partial class Form1 : Form
    {
        private Rectangle circleBounds;
        private bool isDrawingCircle = false;
        private int delay;
        private Timer timer;
        private List<Label> labelList = new List<Label>();
        private List<Label> foundLabels = new List<Label>(); // Danh sách lưu trữ các label đã tô viền
        private int currentStepIndex = -1;
        private List<Action> steps = new List<Action>();
        private bool isPaused = false;


        public Form1()
        {
            InitializeComponent();
            timer = new Timer();
            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawingCircle)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawEllipse(pen, circleBounds);
                }
            }
        }

        private Label FindLabelByValue(int value)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Label label && label.Text == value.ToString())
                {
                    return label;
                }
            }
            return null;
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            labelKQ.Visible = true;    // Kích hoạt label phép toán
            labelResult.Visible = true; // Kích hoạt label kết quả
                                        // Lấy giá trị từ TextBox
            if (int.TryParse(textBoxInput.Text, out int inputValue))
            {
                int a = inputValue;
                int result = a % 20;
                labelResult.Text = $"{result}";
                labelKQ.Text = $"{a} % 20 =";

                // Tìm Label có giá trị tương ứng
                Label targetLabel = FindLabelByValue(result);
                delay = 201
                    - trackBarDelay.Value;


                if (targetLabel != null)
                {

                    // Di chuyển vòng tròn tới vị trí của `targetLabel` với tốc độ từ TrackBar
                    MoveCircleToLabelInsert(targetLabel, a);
                  
                }
                else
                {
                    MessageBox.Show("Không tìm thấy Label có giá trị phù hợp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                // Ẩn label chứa phép toán và label kết quả sau khi thực hiện xong

            }
            else
            {
                MessageBox.Show("Vui lòng nhập một số nguyên hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private async void MoveCircleToLabelInsert(Label targetLabel, int inputValue)
        {
            int padding = 5;
            int startX = labelResult.Left - padding;
            int startY = labelResult.Top - padding;
            int targetX = targetLabel.Left - padding;
            int targetY = targetLabel.Top - padding;
            int width = labelResult.Width + 2 * padding;
            int height = labelResult.Height + 2 * padding;

            int steps = 50;
            isDrawingCircle = true;

            for (int i = 0; i <= steps; i++)
            {
                // Tính vị trí hiện tại của vòng tròn dựa trên số bước
                int currentX = startX + (targetX - startX) * i / steps;
                int currentY = startY + (targetY - startY) * i / steps;

                // Cập nhật vị trí và kích thước của vòng tròn để vẽ lại trong `Paint`
                circleBounds = new Rectangle(currentX, currentY, width, height);

                // Yêu cầu vẽ lại Form
                this.Invalidate();

                // Đợi theo độ trễ từ TrackBar
                await Task.Delay(delay);
            }

            isDrawingCircle = false;
            this.Invalidate(); // Xóa vòng tròn khi di chuyển xong
            CreateNewLabelAboveTarget(targetLabel, inputValue);

        }

        private async void MoveCircleToLabelFind(Label targetLabel, int inputValue)
        {
           
            List<Label> columnLabels = new List<Label>();
            foreach (Control control in this.Controls)
            {
                if (control is Label label && label.Left == targetLabel.Left&& label.BorderStyle == BorderStyle.FixedSingle) // Cùng cột
                {
                    columnLabels.Add(label);
                }
            }

            // Xóa viền của tất cả các Label đã tìm thấy trước đó
            ResetFoundLabels();

            // Di chuyển vòng tròn đến vị trí của targetLabel (key)
            int padding = 5;
            int startX = labelResult.Left - padding;
            int startY = labelResult.Top - padding;
            int targetX = targetLabel.Left - padding; 
            int targetY = targetLabel.Top - padding;
            int width = labelResult.Width + 2 * padding;
            int height = labelResult.Height + 2 * padding;

            int steps = 50;
            isDrawingCircle = true;

            for (int i = 0; i <= steps; i++)
            {
                // Tính vị trí hiện tại của vòng tròn dựa trên số bước
                int currentX = startX + (targetX - startX) * i / steps;
                int currentY = startY + (targetY - startY) * i / steps;

                // Cập nhật vị trí và kích thước của vòng tròn để vẽ lại trong `Paint`
                circleBounds = new Rectangle(currentX, currentY, width, height);

                // Yêu cầu vẽ lại Form
                this.Invalidate();

                // Đợi theo độ trễ từ TrackBar
                await Task.Delay(delay);
            }

            isDrawingCircle = false;
            this.Invalidate(); // Xóa vòng tròn khi di chuyển xong

            bool found = false;

            // Duyệt qua từng Label từ dưới lên trên
            foreach (Label label in columnLabels)
            {
                // Tô màu cho Label hiện tại để báo hiệu đang kiểm tra
                label.ForeColor = Color.Red;  // Đổi màu chữ nếu cần
                label.BackColor = Color.Gray;

                // Đợi một chút để người dùng thấy được Label đang xét
                await Task.Delay(delay+500);

                // So sánh giá trị của Label hiện tại với giá trị cần tìm
                if (label.Text == inputValue.ToString())
                {
                    // Tô màu cho Label tìm thấy
                    label.ForeColor = Color.Red;  // Đổi màu chữ nếu cần
                    label.BackColor = Color.Gray;
                    foundLabels.Add(label);
                    found = true;
                    break; // Dừng lại khi tìm thấy
                }
                else
                {
                    // Trả về màu mặc định nếu không khớp

                    label.ForeColor = Color.Blue;
                    label.BackColor = Color.LightGreen;  // Đặt màu nền cho Label
                    label.BorderStyle = BorderStyle.FixedSingle; // Thêm viền cho Label
                    label.TextAlign = ContentAlignment.MiddleCenter;
                }
            }

            // Nếu không tìm thấy, hiển thị thông báo
            if (!found)
            {
                MessageBox.Show("Không tìm thấy Label có giá trị phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private async void MoveCircleToLabelDelete(Label targetLabel, int inputValue)
        {
           
            List<Label> columnLabels = new List<Label>();
            foreach (Control control in this.Controls)
            {
                if (control is Label label && label.Left == targetLabel.Left && label.BorderStyle == BorderStyle.FixedSingle) // Cùng cột
                {
                    columnLabels.Add(label);
                }
            }

            // Xóa viền của tất cả các Label đã tìm thấy trước đó
            ResetFoundLabels();

            // Di chuyển vòng tròn đến vị trí của targetLabel (key)
            int padding = 5;
            int startX = labelResult.Left - padding;
            int startY = labelResult.Top - padding;
            int targetX = targetLabel.Left - padding;
            int targetY = targetLabel.Top - padding;
            int width = labelResult.Width + 2 * padding;
            int height = labelResult.Height + 2 * padding;

            int steps = 50;
            isDrawingCircle = true;

            for (int i = 0; i <= steps; i++)
            {
                // Tính vị trí hiện tại của vòng tròn dựa trên số bước
                int currentX = startX + (targetX - startX) * i / steps;
                int currentY = startY + (targetY - startY) * i / steps;

                // Cập nhật vị trí và kích thước của vòng tròn để vẽ lại trong `Paint`
                circleBounds = new Rectangle(currentX, currentY, width, height);

                // Yêu cầu vẽ lại Form
                this.Invalidate();

                // Đợi theo độ trễ từ TrackBar
                await Task.Delay(delay);
            }

            isDrawingCircle = false;
            this.Invalidate(); // Xóa vòng tròn khi di chuyển xong

            bool found = false;

            // Duyệt qua từng Label từ dưới lên trên
            foreach (Label label in columnLabels)
            {
                // Tô màu cho Label hiện tại để báo hiệu đang kiểm tra
                label.ForeColor = Color.Red;  // Đổi màu chữ nếu cần
                label.BackColor = Color.Gray;

                // Đợi một chút để người dùng thấy được Label đang xét
                await Task.Delay(delay + 500);

                // So sánh giá trị của Label hiện tại với giá trị cần tìm
                if (label.Text == inputValue.ToString())
                {
                    // Tô màu cho Label tìm thấy
                    label.ForeColor = Color.Red;  // Đổi màu chữ nếu cần
                    label.BackColor = Color.Gray;
                    foundLabels.Add(label);
                    found = true;
                    break; // Dừng lại khi tìm thấy
                }
                else
                {
                    // Trả về màu mặc định nếu không khớp

                    label.ForeColor = Color.Blue;
                    label.BackColor = Color.LightGreen;  // Đặt màu nền cho Label
                    label.BorderStyle = BorderStyle.FixedSingle; // Thêm viền cho Label
                    label.TextAlign = ContentAlignment.MiddleCenter;
                }
            }

            // Nếu không tìm thấy, hiển thị thông báo
            if (!found)
            {
                MessageBox.Show("Không tìm thấy Label có giá trị phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            string valueToDelete = textBoxDelete.Text; // Giả sử giá trị nhập từ textbox

            // Kiểm tra nếu giá trị nhập không rỗng
            if (!string.IsNullOrEmpty(valueToDelete))
            {
                // Gọi phương thức xóa label
                DeleteLabel(valueToDelete);

                // Cập nhật giao diện sau khi xóa
                this.Invalidate(); // Vẽ lại form sau khi thay đổi
            }
            else
            {
                MessageBox.Show("Vui lòng nhập giá trị cần xóa.");
            }

        }
        private bool ValueAlreadyExists(Label targetLabel, int value)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Label label &&
                    label.Left == targetLabel.Left && // Cùng cột với targetLabel
                    label.Text == value.ToString()&& label.BorderStyle == BorderStyle.FixedSingle)   // Giá trị trùng nhau
                {
                    return true; // Giá trị đã tồn tại
                }
            }
            return false; // Giá trị chưa tồn tại
        }
        private void CreateNewLabelAboveTarget(Label targetLabel, int value)
        {
            // Kiểm tra giá trị đã tồn tại trên cột chưa
            if (ValueAlreadyExists(targetLabel, value))
            {
                MessageBox.Show($"Giá trị {value} đã tồn tại trong cột!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Dừng lại, không tạo Label mới
            }
            // Tính vị trí bắt đầu cho label mới
            int offset = 20;  // Khoảng cách giữa các Label mới
            int newYPosition = targetLabel.Top - offset;

            // Duyệt qua các label hiện tại và kiểm tra có label nào đã ở trên targetLabel không
            foreach (Control control in this.Controls)
            {
                if (control is Label label && label != targetLabel)
                {
                    // Nếu có một label ở vị trí trên targetLabel, điều chỉnh vị trí newYPosition cao hơn
                    if (label.Left == targetLabel.Left && label.Top == newYPosition)
                    {
                        newYPosition -= offset;
                    }
                }
            }

            // Tạo một Label mới và đặt tại vị trí trên cùng đã tính toán
            Label newLabel = new Label
            {
                Text = value.ToString(),
                AutoSize = true,
                Location = new Point(targetLabel.Left, newYPosition),
                ForeColor = Color.Blue,
                BackColor = Color.LightGreen,  // Đặt màu nền cho Label
                BorderStyle = BorderStyle.FixedSingle, // Thêm viền cho Label
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(newLabel);
            labelKQ.Visible = false;   // Ẩn label phép toán
            labelResult.Visible = false;   // Ẩn label kết quả

        }
        private void button1_Click(object sender, EventArgs e)
        {
            labelKQ.Visible = true;    // Kích hoạt label phép toán
            labelResult.Visible = true; // Kích hoạt label kết quả
                                        // Lấy giá trị từ TextBox
            if (int.TryParse(textBoxFind.Text, out int inputValue))
            {
                int a = inputValue;
                int result = a % 20;
                labelResult.Text = $"{result}";
                labelKQ.Text = $"{a} % 20 =";

                // Tìm Label có giá trị tương ứng
                Label targetLabel = FindLabelByValue(result);
                delay = 201
                    - trackBarDelay.Value;


                if (targetLabel != null)
                {

                    // Di chuyển vòng tròn tới vị trí của `targetLabel` với tốc độ từ TrackBar
                    MoveCircleToLabelFind(targetLabel, a);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy giá trị phù hợp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                // Ẩn label chứa phép toán và label kết quả sau khi thực hiện xong

            }
            else
            {
                MessageBox.Show("Vui lòng nhập một số nguyên hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Lấy giá trị từ TextBox tìm kiếm
           
        }

        private void ResetFoundLabels()
        {
            // Xóa viền của tất cả các label đã tìm thấy trước đó
            foreach (var label in foundLabels)
            {

                label.ForeColor = Color.Blue;  // Khôi phục màu chữ ban đầu
                label.BackColor = Color.LightGreen;
            }
            foundLabels.Clear(); // Xóa danh sách các label đã tìm thấy
        }
        private void DeleteLabel(string valueToDelete)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label label && label.Text == valueToDelete && label.BorderStyle == BorderStyle.FixedSingle)
                {
                    // Xóa label nếu tìm thấy
                    this.Controls.Remove(label);
                    label.Dispose(); // Giải phóng bộ nhớ nếu cần thiết
                    break; // Dừng vòng lặp nếu đã tìm thấy label cần xóa
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            labelKQ.Visible = true;    // Kích hoạt label phép toán
            labelResult.Visible = true; // Kích hoạt label kết quả
                                        // Lấy giá trị từ TextBox
            if (int.TryParse(textBoxDelete.Text, out int inputValue))
            {
                int a = inputValue;
                int result = a % 20;
                labelResult.Text = $"{result}";
                labelKQ.Text = $"{a} % 20 =";

                // Tìm Label có giá trị tương ứng
                Label targetLabel = FindLabelByValue(result);
                delay = 201
                    - trackBarDelay.Value;


                if (targetLabel != null)
                {

                    // Di chuyển vòng tròn tới vị trí của `targetLabel` với tốc độ từ TrackBar
                    MoveCircleToLabelDelete(targetLabel, a);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy Label có giá trị phù hợp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                // Ẩn label chứa phép toán và label kết quả sau khi thực hiện xong

            }
            else
            {
                MessageBox.Show("Vui lòng nhập một số nguyên hợp lệ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Lấy giá trị từ TextBox tìm kiếm
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
