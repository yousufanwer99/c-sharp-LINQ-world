using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace LinqPractice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var employees = new List<Employee>
            {
                new Employee { Id = 1, GenderId=1, EmployeeTypeId=2, Name = "Ali", Department = "HR", Salary = 50000, JoiningDate = new DateTime(2018, 6, 15) },
                new Employee { Id = 2, GenderId=2, EmployeeTypeId=1, Name = "Sara", Department = "IT", Salary = 80000, JoiningDate = new DateTime(2020, 1, 20) },
                new Employee { Id = 3, GenderId=1, EmployeeTypeId=2, Name = "John", Department = "Finance", Salary = 75000, JoiningDate = new DateTime(2017, 9, 10) },
                new Employee { Id = 4, GenderId=2, EmployeeTypeId=1, Name = "Ayesha", Department = "HR", Salary = 60000, JoiningDate = new DateTime(2019, 3, 25) },
                new Employee { Id = 5, GenderId=1, EmployeeTypeId=2, Name = "Michael", Department = "IT", Salary = 400000, JoiningDate = new DateTime(2021, 5, 30) },
                new Employee { Id = 6, GenderId=2, EmployeeTypeId=1, Name = "Emily", Department = "Finance", Salary = 70000, JoiningDate = new DateTime(2016, 11, 5) },
                new Employee { Id = 7, GenderId=1, EmployeeTypeId=1, Name = "David", Department = "IT", Salary = 85000, JoiningDate = new DateTime(2019, 7, 15) },
                new Employee { Id = 8, GenderId=2, EmployeeTypeId=2, Name = "Sophia", Department = "HR", Salary = 55000, JoiningDate = new DateTime(2020, 8, 18) },
                new Employee { Id = 8, GenderId=2, EmployeeTypeId=1, Name = "Kinrah", Department = "HR", Salary = 300000, JoiningDate = new DateTime(2025, 1, 1) },
            };
            var gender = new List<Gender>
            {
                new Gender{ Id=1,Title="Male"},
                new Gender{ Id=2,Title="Fe-Male"}
            };

            var employeeType = new List<EmployeeType>
            {
                new EmployeeType{ Id=1,Title="full-time"},
                new EmployeeType{ Id=2,Title="part-time"}
            };

            #region #Linq-level-1

            //Find all employees in the "IT" department.
            var data = employees.Where(x => x.Department == "IT").Select(y => GetEmployee(y));

            //Retrieve names of employees who have a salary greater than 70,000.
            data = employees.Where(x => x.Salary > 70000).Select(y => GetEmployee(y));

            //Select the names and salaries of all employees.
            data = employees.Select(y => GetEmployee(y));

            //Find the employee with the highest salary.
            //WAY: 1
            var highestSalary = employees.Max(x => x.Salary);
            var highestPayingEmpliyee = employees.First(x => x.Salary == highestSalary);
            //WAY: 2
            highestPayingEmpliyee = employees.OrderByDescending(x => x.Salary).Take(1).First();


            //Calculate the average salary of employees in the "Finance" department.
            var avgSalaryOfFinanceDept = employees.Where(x => x.Department == "Finance").Average(x => x.Salary);

            //Get the total salary paid to all employees in the company.
            var totalSalary = employees.Sum(x => x.Salary);

            //Find employees who joined before January 1, 2020.
            var dateCriteria = new DateTime(2020, 1, 1);
            data = employees.Where(x => x.JoiningDate < dateCriteria).Select(y => GetEmployee(y));

            //Sort all employees by their salary in descending order.
            data = employees.OrderByDescending(x => x.Salary).Select(y => GetEmployee(y));


            var top3HighestPayingEmployee = employees.
                                            OrderByDescending(x => x.Salary)
                                            .Select(y => GetEmployee(y)).Take(3);


            //Retrieve employees sorted by their joining date(oldest first).
            data = employees.OrderBy(x => x.JoiningDate).Select(y => GetEmployee(y));

            //Group employees by department and count the number of employees in each department.
            var g1 = employees.GroupBy(x => x.Department)
                              .Select(grp => new { _groupName = grp.Key, _count = grp.Count() });
            foreach (var item in g1)
            {
                //Console.WriteLine($"{item._groupName}: {item._count}");
            }

            //Find the average salary of employees in each department.
            var g2 = employees.GroupBy(x => x.Department)
                             .Select(grp => new { _groupName = grp.Key, _avg = grp.Average(p => p.Salary) });
            foreach (var item in g2)
            {
                //Console.WriteLine($"{item._groupName}: {item._avg}");
            }

            //Group employees by department and get the highest salary in each group.
            var g3 = employees.GroupBy(x => x.Department)
                            .Select(grp => new { _groupName = grp.Key, _highestSalary = grp.Max(p => p.Salary) });
            foreach (var item in g3)
            {
                //Console.WriteLine($"{item._groupName}: {item._highestSalary}");
            }

            //Find employees who have been working for more than 5 years.
            Console.WriteLine("Find employees who have been working for more than 5 years");
            var fiveYearsAgo = DateTime.Now.AddYears(-5);
            
            //Normal Way
            //data = employees.Where(x => x.JoiningDate < fiveYearsAgo).Select(y => GetEmployee(y));
            
            //Passing Predicate
            Func<Employee, bool> validateJoiningDate = (employee) => employee.JoiningDate < fiveYearsAgo;
            data = employees.Where(x => validateJoiningDate(x)).Select(y => GetEmployee(y));

            foreach (var item in data)
            {
                //Console.WriteLine($"{item.Name}: {item.JoiningDate}");
            }


            //Get the list of departments and a flag indicating
            //whether each department has employees earning above 80,000.
            var g4 = employees.GroupBy(x => x.Department)
                            .Select
                            (
                                grp => new { _groupName = grp.Key, _is80Above = grp.Count(p => p.Salary > 80000) > 0 ? "Above 80 Pay Scale" : "Below 80 Pay Scale" }
                            );
            foreach (var item in g4)
            {
                //Console.WriteLine($"{item._groupName}: {item._is80Above}");
            }


            //Check if any employee earns less than 50,000.
            var _anyEmployeeBelo50K = employees.Any(x => x.Salary < 50000);


            //Find the second-highest-paid employee in the company.
            var challengeOne = employees.OrderByDescending(x => x.Salary).Take(2).OrderBy(y => y.Salary).Take(1);


            //Get the total salary of employees in each department, but only include departments
            //where the total salary exceeds 150,000.
            var g5 = employees
                .GroupBy(x => x.Department)
                .Select
                (
                    grp => new { _groupName = grp.Key, _totalSalary = grp.Sum(p => p.Salary) }
                )
                .Where(grp => grp._totalSalary > 150000); 
            foreach (var item in g5)
            {
                //Console.WriteLine($"{item._groupName}: {item._totalSalary}");
            }


            //Retrieve employees whose names start with "A" and sort them by salary.
            var g6 = employees.Where(x => x.Name.StartsWith("A")).OrderBy(x => x.Salary);

            #endregion

            #region #Linq-Joins


            var joindData = employees
                .Join
                (
                    gender,
                    e => e.GenderId,
                    g => g.Id,
                    (e, g) => new { e.Name, e.EmployeeTypeId, Gender = g.Title }
                ) //as e
                .Join
                (
                    employeeType,
                    e => e.EmployeeTypeId,
                    et => et.Id,
                    (e, et) => new { e.Name, e.Gender, EmployeeType = et.Title }

                )
                .GroupBy(tbl => tbl.Gender)
                .Select
                (
                   groupedTbl => new { genderName = groupedTbl.Key, _count = groupedTbl.Count() }
                );
            foreach (var item in joindData)
            {
                //Console.WriteLine($"{item.Name} - {item.Gender} - {item.EmployeeType}");
                Console.WriteLine($"{item.genderName} - {item._count}");
            }
            #endregion

        }
        static Employee GetEmployee(dynamic _emp)
        {
            var emp = new Employee();
            emp.Name = _emp.Name;
            emp.Salary = _emp.Salary;
            emp.Department = _emp.Department;
            emp.JoiningDate = _emp.JoiningDate;
            return emp;
        }

    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public int GenderId { get; set; }
        public int EmployeeTypeId { get; set; }

    }
    public abstract class CommanFields
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class Gender : CommanFields
    {
    }
    public class EmployeeType : CommanFields
    {
    }
}