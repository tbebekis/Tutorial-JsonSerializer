namespace WinFormsApp
{

    //[JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        None,
        Pending,
        InProgress,
        AllCompleted
    }

    public class Part
    {
        public string Code { get; set; } = "123";
        public decimal Amount { get; set; } = 12.34M;
        public bool IsCompleted { get; set; } = true;
    }

    public class Job
    {
        public Job()
        {
        }

        static public List<Job> GetList()
        {
            List<Job> Result = new();

            Job M1 = new Job();
            M1.Name = "Model 1";
            M1.Status = Status.InProgress;
            M1.Active = true;
            M1.Parts.Add(new Part() { Code = "001", Amount = 1.2M, IsCompleted = true});
            M1.Parts.Add(new Part() { Code = "002", Amount = 3.4M, IsCompleted = false });
            M1.Properties["John"] = new { Name = "John", Age = 40};
            M1.Properties["NiceCar"] = new { Model = "Volvo", Year = 2025 };


            Job M2 = new Job();
            M2.Name = "Model 2";
            M2.Status = Status.AllCompleted;
            M2.Active = true;
            M2.Parts.Add(new Part() { Code = "003", Amount = 5.6M, IsCompleted = false });
            M2.Parts.Add(new Part() { Code = "004", Amount = 7.8M, IsCompleted = true });

            Result.AddRange(new Job[] { M1, M2});

            return Result;
        }

        public int Id { get; set; } = 1;
        public string Name { get; set; } = "Model";
        public Status Status { get; set; } = Status.Pending;
        public bool Active { get; set; } = false;
        public List<Part> Parts { get; set; } = new();    
        public Dictionary<string, object> Properties { get; set; } = new();
        public DateTime DT { get; set; } = DateTime.Now;
    }
}
