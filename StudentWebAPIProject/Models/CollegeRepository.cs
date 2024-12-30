namespace StudentWebAPIProject.Models
{
    public static class CollegeRepository
    {
        public static List<Student> Students = new List<Student>()
        {
            new Student
            {
                id = 1,
                name = "Ramesh",
                age = 20,
                address = "Bangalore",
                email = "ramesh@test.com"
            },
            new Student
            {
                id = 2,
                name = "Rajesh",
                age = 20,
                address = "Bangalore",
                email = "rajesh@test.com"
            }
        };
    }
}
