namespace StudentWebAPIProject.Models
{
    public static class CollegeRepository
    {
        public static List<Student> Students = new List<Student>()
        {
            new Student
            {
                Id = 1,
                Name = "Ramesh",
                DOB = new DateTime(2000, 10, 05),
                Address = "Bangalore",
                Email = "ramesh@test.com",
                AddmissionDate = DateTime.Today
            },
            new Student
            {
                Id = 2,
                Name = "Rajesh",
                DOB = new DateTime(2000, 10, 05),
                Address = "Bangalore",
                Email = "rajesh@test.com",
                AddmissionDate = DateTime.Today
            }
        };
    }
}
