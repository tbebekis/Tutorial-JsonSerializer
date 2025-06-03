
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        void FormInitialize()
        {
            btnClear.Click += (o, e) => LogClear();
            btnPascalCase.Click += (o, e) => Test_PascalCase();
            btnCamelCase.Click += (o, e) => Test_CamelCase();
            btnPopulateObject.Click += (o, e) => Test_PopulateObject();
            btnFormat.Click += (o, e) => Test_FormatJson();
            btnToDynamic.Click += (o, e) => Test_ToDynamic();
            btnExcludeProperties.Click += (o, e) => Test_ExcludeProperties();
        }

        void Test_PascalCase()
        {
            LogClear();
            List<Model> List = Model.GetList();
            string JsonText = NetJson.Serialize(List);
            Log(JsonText);

        }
        void Test_CamelCase()
        {
            LogClear();
            List<Model> List = Model.GetList();
            string JsonText = NetJson.Serialize(List, NetJson.CreateOptions(CameCase: true));
            Log(JsonText);

        }
        void Test_PopulateObject()
        {
            Part P = new Part();
            string JsonText = NetJson.Serialize(P);
            P.Amount = 100;


            NetJson.PopulateObject(P, JsonText);

            Model M1 = new Model();
            M1.Name = "Model 1";
            M1.Status = Status.Pending;
            M1.Active = true;
            M1.Parts.Add(new Part() { Code = "001", Amount = 1.2M, IsCompleted = true });
            M1.Parts.Add(new Part() { Code = "002", Amount = 3.4M, IsCompleted = false });
            M1.Properties["John"] = new { Name = "John", Age = 40 };
            M1.Properties["NiceCar"] = new { Model = "Volvo", Year = 2025 };

            JsonText = NetJson.Serialize(M1);

            M1.Properties["Key"] = "Value";
            M1.Status = Status.InProgress;

            NetJson.PopulateObject(M1, JsonText);

            string S = $@"
INITIAL JSON
---------------------------------------
{JsonText}

---------------------------------------
Change some values to Model 
and PopulateObject() from INITIAL JSON
---------------------------------------

NEW JSON
---------------------------------------

{NetJson.Serialize(M1)}

";


            Log(S);


        }
        void Test_FormatJson()
        {
            Model M1 = new Model();
            string JsonText = NetJson.Serialize(M1, NetJson.CreateOptions(CameCase: false, Formatted: false));

            string FormattedJsonText = NetJson.Format(JsonText);

            string S = $@"
UN-FORMATTED

{JsonText}

FORMATTED

{FormattedJsonText}
";
            Log(S);
        }
        void Test_ToDynamic()
        {
            Part P = new Part();
            string JsonText = NetJson.Serialize(P);

            dynamic D = NetJson.ToDynamic(JsonText);
            JsonObject JO = D as JsonObject;
 
            string S = $@"
INITIAL JSON
---------------------------------------
{JsonText}

---------------------------------------
dynamic D = NetJson.ToDynamic(JsonText);
JsonObject JO = D as JsonObject;
JsonText = JO.ToString();
---------------------------------------

{JO.ToString()}

";
            Log(S);

        }
        void Test_ExcludeProperties()
        {
            Part P = new Part();
            string JsonText = NetJson.Serialize(P);

            string S = $@"
INITIAL JSON
---------------------------------------
{JsonText}

---------------------------------------
Exclude Code and IsCompleted properties 
---------------------------------------

{NetJson.Serialize(P, new string[] {"Code", "IsCompleted" })}

";
            Log(S);
        }

        void LogClear()
        {
            edtLog.Clear();
        }
        void Log(string Text)
        {
            edtLog.Clear();
            edtLog.AppendText(Text);

            if (edtLog.Text.Length > 0)
            {
                edtLog.SelectionStart = edtLog.Text.Length;
                edtLog.ScrollToCaret();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (!DesignMode)
                FormInitialize();
            base.OnShown(e);
        }

        public MainForm()
        {
            InitializeComponent();
        }
 
    }
}
