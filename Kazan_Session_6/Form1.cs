using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

namespace Kazan_Session_6
{
    public partial class Form1 : Form
    {
        KazanSession6Entities entities;
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedIndex)
            {
                case 0:
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    break;
                case 1:
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("hi-IN");
                    break;
                case 3:
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pa-IN");
                    break;
                default:
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    break;
            }

            this.Controls.Clear();
            InitializeComponent();

            Form1_Load(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            entities = new KazanSession6Entities();
            dataGridView1.DataSource = entities.OrderItems
                .GroupBy(x => new {
                    x.Order.EmergencyMaintenance.Asset.DepartmentLocation.Department.Name,
                    x.Order.Date.Month
                }).Select(x => new
                {
                    Department = x.Key.Name,
                    Month = x.Key.Month,
                    Count = x.Sum(g => g.UnitPrice * g.Amount)
                }).ToList();

            //Pie Chart
            chart1.DataSource = entities.OrderItems
                 .GroupBy(x => new {
                     x.Order.EmergencyMaintenance.Asset.DepartmentLocation.Department.Name
                 }).Select(x => new
                 {
                     Department = x.Key.Name,
                     Count = x.Sum(g => g.UnitPrice * g.Amount)
                 }).ToList();

            chart1.Series["Series1"].XValueMember = "Department";
            chart1.Series["Series1"].YValueMembers = "Count";
            chart1.Series["Series1"].IsValueShownAsLabel = true;

            //Pie Chart
            chart2.DataSource = entities.OrderItems
                 .GroupBy(x => new {
                     x.Order.EmergencyMaintenance.Asset.DepartmentLocation.Department.Name,
                     x.Order.Date.Year
                 }).Select(x => new
                 {
                     Department = x.Key.Name,
                     Month = x.Key.Year,
                     Count = x.Sum(g => g.UnitPrice * g.Amount)
                 }).ToList();

            chart2.Series["Series1"].XValueMember = "Month";
            chart2.Series["Series1"].YValueMembers = "Count";
//            chart2.Series["Series1"].IsValueShownAsLabel = true;
            chart2.ChartAreas["ChartArea2"].AxisX.ScrollBar.Enabled = true;



            //DataGrid2
            dataGridView2.DataSource = entities.OrderItems
                .GroupBy(x => new {
                    x.Part.Name,
                    x.Order.Date.Month
                })
                .Select(x => new
                {
                    PartName = x.Key.Name,
                    Month = x.Key.Month,
                    PartSum = x.Max(y => y.UnitPrice * y.Amount),
                    //x.Max(g => g.UnitPrice * g.Amount),
                    PartCount = x.Max(g => g.UnitPrice)
                }).ToList();

            entities.OrderItems
                .GroupBy(x => new {
                    x.Part.Name,
                    x.Order.Date.Month
                }).Select(x => new
                {
                    PartName = x.Key.Name,
                    Month = x.Key.Month,
                    PartCount = x.Max(g => g.UnitPrice)
                }).ToList();


            //Datagridview 3
            entities.OrderItems
                .GroupBy(x => new {
                    x.Order.EmergencyMaintenance.Asset.AssetName,
                    x.Order.Date.Month
                }, (key,group) => new {
                    Asset_Name = key.AssetName
                }).ToList();

            //Setting datagridview1 one data
            var list= entities.OrderItems
                .Select(a => new
                {
                    Month = a.Order.Date.Month,
                    Year = a.Order.Date.Year,
                    Amount = a.UnitPrice * a.Amount,
                    Department = a.Order.EmergencyMaintenance.Asset.DepartmentLocation.Department.Name
                }).GroupBy(x => new
                {
                    year = x.Year, 
                    month = x.Month,
                    dept = x.Department
                }, (a,b) => new
                {
                    date = a.month + "-" + a.year,
                    amount = b.Sum(c=> c.Amount),
                    department =  a.dept 
                }).ToList();

            
            var months = entities.OrderItems
             .Select(a => new
             {
                 Month = a.Order.Date.Month,
                 Year = a.Order.Date.Year
                 
             }).OrderBy(x=> x.Year).OrderBy(x => x.Month).GroupBy(x => new
             {
                 year = x.Year,
                 month = x.Month
                 
             }, (a, b) => new
             {
                 date = a.month + "-" + a.year                 
                
             }).Distinct().ToList();
            
            DataGridViewTextBoxColumn cell1 = new DataGridViewTextBoxColumn();
            cell1.Name = "Department/Month";
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            cell1});
            foreach (var m in months)
            {
                DataGridViewTextBoxColumn cell = new DataGridViewTextBoxColumn();
                cell.Name = m.date;
                Console.WriteLine("Date: " + m.date);
                this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            cell});
            }
            var department = entities.OrderItems
             .GroupBy(x => new
             {
                 dept = x.Order.EmergencyMaintenance.Asset.DepartmentLocation.Department.Name

             }).Select(a => new
             {
                 Department = a.Key.dept

             }).OrderBy(x => x.Department).Distinct().ToList();
            foreach (var d in department)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView3);
                row.Cells[0].Value = d.Department;
                Console.WriteLine("Date: " + d.Department);
                this.dataGridView3.Rows.Add(row);
            }

            foreach(var l in list)
            {
                for(int i = 1; i < dataGridView3.Columns.Count; i++)
                {
                    Console.WriteLine("Row count: " + dataGridView3.Rows.Count);
                    for (int j = 1; j < dataGridView3.Rows.Count; j++)
                    {

                        if (dataGridView3.Rows[j].Cells[0].Value != null) {
                        if (l.date == dataGridView3.Columns[i].Name && l.department == dataGridView3.Rows[j].Cells[0].Value.ToString())
                        {
                            dataGridView3.Rows[j].Cells[i].Value = l.amount;
                        }
                    }
                    }
                }
            }

            //Select(x => new
            //{
            //     Asset_Name = x.Key.AssetName,
            //     Month = x.Key.Month,
            //    Department = x.Select(g => g.Order.EmergencyMaintenance.Asset.DepartmentLocation.Department.Name),
            //   EM_EndDate = x.Select(g => g.Order.EmergencyMaintenance.EMEndDate)
            //})
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
