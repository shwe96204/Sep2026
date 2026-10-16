using System;
using Sep2026.Database_ADO.CommonFunction;

namespace Sep2026.Database_ADO.Sutdent_CRUD;

public class SutdentRepo
{
    StudentService stdService = new StudentService();
    public void Start()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Student Management System");
            Console.WriteLine("1. Create student");
            Console.WriteLine("2. List students");
            Console.WriteLine("3. Update student");
            Console.WriteLine("4. Delete students");
            Console.WriteLine("5. Exist");
            Console.Write("Choose an option: ");
            string? option = Console.ReadLine();
            Console.WriteLine();

            if (string.IsNullOrEmpty(option))
            {
                break;
            }

            switch (option)
            {
                case "1":
                {
                    var stdNo = ConsoleHelper.ReadText("Please enter student number: ");
                    var name = ConsoleHelper.ReadText("Please enter student name: ");
                    var classNo = ConsoleHelper.ReadText("Please enter class no: ");
                    stdService.CreateStudent(stdNo, name, classNo);
                    break;
                }
                case "2":
                {
                    stdService.ShowAllStudent();
                    break;
                }
                case "3":
                {
                    stdService.ShowAllStudent();
                    var id = ConsoleHelper.ReadNumber("Please enter student id: ");
                    if (!stdService.IsStudentExist(id))
                    {
                        ConsoleHelper.ShowErrorMessage("Student not found.");
                        break;
                    }

                    var stdNo = ConsoleHelper.ReadText("Please enter student number: ");
                    var name = ConsoleHelper.ReadText("Please enter student name: ");
                    var classNo = ConsoleHelper.ReadText("Please enter class no: ");
                    stdService.EditStudent(id, stdNo, name, classNo);
                    break;
                }
                case "4":
                {
                    stdService.ShowAllStudent();
                    var id = ConsoleHelper.ReadNumber("Please enter student id: ");
                    stdService.DeleteStudent(id);
                    break;
                }
                case "5":
                {
                    running = false;
                    Console.WriteLine("App Exit...");
                    break;
                }
                default:
                {
                    ConsoleHelper.ShowInfoMessage("Please choose an option between 1 and 5: ");
                    break;
                }
            }

            Console.WriteLine();
            
        }

        Console.WriteLine();
        
    }
}
