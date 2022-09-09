using Labb2EFLinQ.Data;
using Labb2EFLinQ.Handlers;
using Labb2EFLinQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Labb2EFLinQ
{
    public class Program
    {
        static private List<Class> classesList = new List<Class>();
        static private List<Course> coursesList = new List<Course>();
        static private List<SchoolSchedule> schoolSchedulesList = new List<SchoolSchedule>();
        static private List<Student> studentsList = new List<Student>();
        static private List<Teacher> teachersList = new List<Teacher>();

        static private bool closeProgram;

        static void Main(string[] args)
        {
            Start();
            do
            {
                MainMenu();
            }
            while (closeProgram == false);
        }



        static void Start()
        {
            Console.WriteLine("School Schedule\n\nPlease wait while loading..");
            GetCoursesList();
            GetClassesList();
            GetShoolSchedulesList();
            GetStudentsList();
            GetTeachersList();
        }

        // This is the main menu
        static void MainMenu()
        {
            string prompt = "School Schedule - Main Menu\n";
            string[] options =
                {
                    "View teachers for a given course",
                    "View all students with their teachers for a given course",
                    "View all students with their respective teachers",
                    "Rename a course",
                    "Change a students teacher of a given course",
                    "Exit program"
                };
            Menu mainMenu = new Menu(prompt, options);
            int selectedIndex = mainMenu.Run();

            try
            {
                switch (selectedIndex)
                {
                    case 0:
                        Console.Clear();
                        Console.WriteLine("School Schedule - Enter course name to see its teachers\n");
                        Console.WriteLine("Enter course name");
                        Console.Write("> ");
                        var courseNameInput = Console.ReadLine();
                        GetTeachersForASpecificCourse(courseNameInput);
                        Console.ReadKey();
                        break;
                    case 1:
                        Console.Clear();
                        Console.WriteLine("School Schedule - Enter course name to see its teachers with their students\n");
                        Console.WriteLine("Enter course name");
                        Console.Write("> ");
                        var courseNameInput2 = Console.ReadLine();
                        GetStudentsWithRespectiveTeachersGivenCourse(courseNameInput2);
                        Console.ReadLine();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("School Schedule - All students with their respective teachers\n");
                        GetStudentsWithRespectiveTeachers();
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("School Schedule - Rename a course\n");
                        Console.WriteLine("Enter the name of the course that you want to rename");
                        Console.Write("> ");
                        var oldCourseName = Console.ReadLine();
                        EditCourseName(oldCourseName);
                        Console.ReadLine();
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine("School Schedule - Change a students teacher of a given course\n");
                        Console.WriteLine("Enter the students name");
                        Console.Write("> ");
                        string studentNameInput = Console.ReadLine();
                        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                        string studentInputName = textInfo.ToTitleCase(studentNameInput.ToLower());

                        // Query for if a user exists or not
                        var checkIfUserExists = from student in studentsList
                                     where student.StudentName.ToLower() == studentNameInput.ToLower()
                                     select new
                                     {
                                         studentName = student.StudentName
                                     };

                        // Query to link together students, teachers and courses
                        var output = from schoolSchedule in schoolSchedulesList
                                 join course in coursesList on schoolSchedule.CourseId equals course.CourseId
                                 join teacher in teachersList on schoolSchedule.TeacherId equals teacher.TeacherId
                                 join student in studentsList on schoolSchedule.StudentId equals student.StudentId
                                 where student.StudentName.ToLower() == studentInputName.ToLower()
                                 select new
                                 {
                                     courseName = course.CourseName,
                                     teacherName = teacher.TeacherName
                                 };

                        if (checkIfUserExists.Count() > 0)
                        {
                            Console.Clear();
                            Console.WriteLine("School Schedule - Change a students teacher of a given course\n");
                            Console.WriteLine($"{studentInputName} takes the following {output.Count()} courses:\n");
                            foreach (var o in output)
                            {
                                Console.WriteLine($"The course {o.courseName} with teacher {o.teacherName}\n");
                            }
                            if (output.Count() != 0)
                            {
                                Console.WriteLine($"Enter the course where you want to change {studentInputName}'s teacher");
                                Console.Write("> ");
                                var courseInputName = Console.ReadLine();
                                EditTeacherForStudent(studentInputName, courseInputName);
                            }
                            else if (output.Count() == 0)
                            {
                                Console.WriteLine($"{studentInputName} doesn't take any courses");
                                Console.ReadLine();
                                break;
                            }
                        }
                        else if (checkIfUserExists.Count() == 0)
                        {
                            Console.WriteLine($"Can't find a student with the name {studentInputName}");
                        }

                        studentsList.Clear();
                        teachersList.Clear();
                        schoolSchedulesList.Clear();

                        GetStudentsList();
                        GetTeachersList();
                        GetShoolSchedulesList();

                        Console.WriteLine("Press any key to continue");
                        
                        Console.ReadLine();
                        break;
                    case 5:
                        closeProgram = true;
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong here..");
            }
        }

        static void GetTeachersForASpecificCourse(string courseNameInput)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string courseName = textInfo.ToTitleCase(courseNameInput.ToLower());

            var output = (from schoolSchedule in schoolSchedulesList
                          join course in coursesList on schoolSchedule.CourseId equals course.CourseId
                          join teacher in teachersList on schoolSchedule.TeacherId equals teacher.TeacherId
                          where course.CourseName.ToLower() == courseNameInput.ToLower()
                          select new
                          {
                              courseName = course.CourseName,
                              teacherName = teacher.TeacherName
                          }).Distinct();

            if (output.Any() == true)
            {
                Console.Clear();
                Console.WriteLine("School Schedule - Enter course name to see its teachers\n");
                foreach (var o in output)
                {
                    Console.WriteLine($"Course: {o.courseName}");
                    Console.WriteLine($"Teacher: {o.teacherName}\n");
                }
            }
            else if (output.Any() != true)
            {
                Console.Clear();
                Console.WriteLine("School Schedule - Enter course name to see its teachers\n");
                Console.WriteLine($"No course named {courseName} was found,\nor that course may not be running at the moment\n\nTry another course");
            }
        }

        static void GetStudentsWithRespectiveTeachersGivenCourse(string courseNameInput2)
        {

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string courseName = textInfo.ToTitleCase(courseNameInput2.ToLower());

            var output = from schoolSchedule in schoolSchedulesList
                         join teacher in teachersList on schoolSchedule.TeacherId equals teacher.TeacherId
                         join course in coursesList on schoolSchedule.CourseId equals course.CourseId
                         join student in studentsList on schoolSchedule.StudentId equals student.StudentId
                         where course.CourseName.ToLower() == courseNameInput2.ToLower()
                         select new
                         {
                             studentName = student.StudentName,
                             teacherName = teacher.TeacherName,
                             courseName = course.CourseName
                         };

            Console.Clear();
            if (output.Any() == true)
            {
                Console.WriteLine("School Schedule - Enter course name to see its teachers with their students\n");
                foreach (var o in output)
                {
                    Console.WriteLine($"Course: {o.courseName}\nTeacher: {o.teacherName}\nStudent: {o.studentName}\n");
                }
            }
            else if (output.Any() != true)
            {
                Console.Clear();
                Console.WriteLine("School Schedule - Enter course name to see its teachers with their students\n");
                Console.WriteLine($"No course named {courseName} was found,\nor that course may not be running at the moment\n\nTry another course");
            }
        }

        static void GetStudentsWithRespectiveTeachers()
        {
            var output = from schoolSchedule in schoolSchedulesList
                         join student in studentsList on schoolSchedule.StudentId equals student.StudentId
                         join teacher in teachersList on schoolSchedule.TeacherId equals teacher.TeacherId
                         join course in coursesList on schoolSchedule.CourseId equals course.CourseId orderby student.StudentId
                         select new
                         {
                             studentName = student.StudentName,
                             teacherName = teacher.TeacherName
                         };

            if (output.Any() == true)
            {
                foreach (var o in output)
                {
                    Console.WriteLine($"\nStudent: {o.studentName}\nTeacher: {o.teacherName}");
                }
            }
            else if (output.Any() != true)
            {
                Console.WriteLine("No results");
            }
        }

        static void EditCourseName(string oldCourseName)
        {
            using SchoolSystemDbContext context = new SchoolSystemDbContext();

            var result = context.Courses.SingleOrDefault(c => c.CourseName.ToLower() == oldCourseName.ToLower());

            TextInfo textInfoOldCourse = CultureInfo.CurrentCulture.TextInfo;
            string courseNameOld = textInfoOldCourse.ToTitleCase(oldCourseName.ToLower());

            if (result != null)
            {
                Console.Clear();
                Console.WriteLine("School Schedule - Rename a course\n");
                Console.WriteLine($"Enter the name that you want to change {courseNameOld} to");
                Console.Write("> ");
                var newCourseName = Console.ReadLine();
                TextInfo textInfoNewCourse = CultureInfo.CurrentCulture.TextInfo;
                string courseNameNew = textInfoNewCourse.ToTitleCase(newCourseName.ToLower());
                result.CourseName = newCourseName;
                context.SaveChanges();
                Console.WriteLine($"\n{courseNameOld} was successfully renamed to {courseNameNew}");
            }
            else if (result == null)
            {
                Console.WriteLine("School Schedule - Rename a course\n");
                Console.WriteLine($"The course {courseNameOld} couldn't be found");
            }
        }

        static void EditTeacherForStudent(string studentInputName, string courseInputName)
        {
            int theStudentId = 0;
            int theCourseId = 0;
            
            TextInfo textInfoCourse = CultureInfo.CurrentCulture.TextInfo;
            string courseName = textInfoCourse.ToTitleCase(courseInputName.ToLower());

            TextInfo textInfoStudent = CultureInfo.CurrentCulture.TextInfo;
            string studentName = textInfoStudent.ToTitleCase(studentInputName.ToLower());
            
            Console.Clear();
            Console.WriteLine("School Schedule - Change a students teacher of a given course\n");
            Console.WriteLine($"Enter the new teacher for the {studentName} in {courseName}");
            Console.Write("> ");
            var newTeacherName = Console.ReadLine();
           
            TextInfo textInfoTeacher = CultureInfo.CurrentCulture.TextInfo;
            string teacherName = textInfoTeacher.ToTitleCase(newTeacherName.ToLower());
            
            using (SchoolSystemDbContext context = new SchoolSystemDbContext())
            {
                var output = from SchoolSchedule in context.SchoolSchedules
                             join student in context.Students on SchoolSchedule.StudentId equals student.StudentId
                             join course in context.Courses on SchoolSchedule.CourseId equals course.CourseId
                             where student.StudentName.ToLower() == studentName.ToLower() && course.CourseName.ToLower() == courseName.ToLower()
                             select new
                             {
                                 studentId = student.StudentId,
                                 courseId = course.CourseId
                             };

                foreach (var o in output)
                {
                    theStudentId = o.studentId;
                    theCourseId = o.courseId;
                }
            };

            using (SchoolSystemDbContext context = new SchoolSystemDbContext())
            {
                var result = context.Teachers.SingleOrDefault(t => t.TeacherName.ToLower() == newTeacherName.ToLower());


                if (result == null)
                {
                    var teacher = new Teacher
                    {
                        TeacherName = newTeacherName
                    };
                    context.Add(teacher);
                    context.SaveChanges();

                    var getNewId = context.Teachers.SingleOrDefault(t => t.TeacherName.ToLower() == newTeacherName.ToLower());
                    var newId = getNewId.TeacherId;
                    var changeTeacher = context.SchoolSchedules.SingleOrDefault(s => s.StudentId == theStudentId && s.CourseId == theCourseId);

                    if (changeTeacher == null)
                    {
                        Console.Clear();
                        Console.WriteLine("School Schedule - Change a students teacher of a given course\n");
                        Console.WriteLine("Something went wrong here..");
                    }
                    else if (changeTeacher != null)
                    {
                        changeTeacher.TeacherId = newId;
                        context.SaveChanges();
                        Console.WriteLine($"\n{teacherName} is now the new teacher");
                    }
                }
                else if (result != null)
                {
                    var newId = result.TeacherId;
                    var changeTeacher = context.SchoolSchedules.SingleOrDefault(s => s.StudentId == theStudentId && s.CourseId == theCourseId);

                    if (changeTeacher == null)
                    {
                        Console.Clear();
                        Console.WriteLine("School Schedule - Change a students teacher of a given course\n");
                        Console.WriteLine("Something went wrong here..");
                    }
                    else if (changeTeacher != null)
                    {
                        changeTeacher.TeacherId = newId;
                        context.SaveChanges();
                        Console.WriteLine($"{teacherName} is now the new teacher");
                    }
                }
            }
        }

        // Get lists
        static void GetClassesList()
        {
            try
            {
                using SchoolSystemDbContext context = new SchoolSystemDbContext();
                var content = context.Classes;

                foreach (Class c in content)
                {
                    classesList.Add(c);
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong here..");
            }
        }

        static void GetCoursesList()
        {
            try
            {
                using SchoolSystemDbContext context = new SchoolSystemDbContext();
                var content = context.Courses;

                foreach (Course c in content)
                {
                    coursesList.Add(c);
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong here..");
            }
        }

        static void GetShoolSchedulesList()
        {
            try
            {
                using SchoolSystemDbContext context = new SchoolSystemDbContext();
                var content = context.SchoolSchedules;

                foreach (SchoolSchedule s in content)
                {
                    schoolSchedulesList.Add(s);
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong here..");
            }
        }


        static void GetStudentsList()
        {
            try
            {
                using SchoolSystemDbContext context = new SchoolSystemDbContext();
                var content = context.Students;

                foreach (Student s in content)
                {
                    studentsList.Add(s);
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong here..");
            }
        }


        static void GetTeachersList()
        {
            try
            {
                using SchoolSystemDbContext context = new SchoolSystemDbContext();
                var DBcontent = context.Teachers;
                foreach (Teacher t in DBcontent)
                {
                    teachersList.Add(t);
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong here..");
            }
        }

        //// DB fillers
        //static void ClassDbFiller()
        //{
        //    using SchoolSystemDbContext context = new SchoolSystemDbContext();
        //    var className = new Class
        //    {
        //        ClassName = "3A"
        //    };

        //    context.Classes.Add(className);
        //    context.SaveChanges();

        //}
        //static void TeacherDbFiller()
        //{
        //    using SchoolSystemDbContext context = new SchoolSystemDbContext();
        //    var teacher = new Teacher
        //    {
        //        TeacherName = "Reidar Nilsen"
        //    };

        //    context.Teachers.Add(teacher);
        //    context.SaveChanges();
        //}
        //static void CourseDbFiller()
        //{
        //    using SchoolSystemDbContext context = new SchoolSystemDbContext();
        //    var course = new Course
        //    {
        //        CourseName = "DevOps"
        //    };
        //    context.Courses.Add(course);
        //    context.SaveChanges();
        //}
        //static void StudentDbFiller()
        //{
        //    using SchoolSystemDbContext context = new SchoolSystemDbContext();
        //    var student = new Student
        //    {
        //        StudentName = "Hannibal Lecter",
        //        ClassId = 2

        //    };

        //    context.Students.Add(student);
        //    context.SaveChanges();
        //}
        //static void SchoolScheduleDbFiller()
        //{
        //    using SchoolSystemDbContext context = new SchoolSystemDbContext();
        //    var studentSchedule = new SchoolSchedule
        //    {
        //        CourseId = 1,
        //        TeacherId = 2,
        //        StudentId = 3,
        //    };
        //    context.SchoolSchedules.Add(studentSchedule);
        //    context.SaveChanges();
        //}
    }
}
