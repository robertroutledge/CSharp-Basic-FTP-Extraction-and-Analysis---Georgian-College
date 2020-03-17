
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Routledge_Assignment_3.Models.Utilities;
using Routledge_Assignment_3.Models;


namespace Routledge_Assignment_3
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Student> students = new List<Student>();
            ///Connects to FTP and builds list of directories
            List<string> directories = FTP.GetDirectory(Constants.FTP.BaseUrl);

            ///Prints out list of directories, using both ToCSV and ToString, per assignment.
            Console.WriteLine("These are the students and their files in the FTP directory:");
            foreach (var directory in directories)
            {
                //Builds a list of students from directories
                ///Answer 4 of Assignment 2.
                Student student = new Student() { AbsoluteUrl = Constants.FTP.BaseUrl };
                student.FromDirectory(directory);
            }
            
            ///extracts data from each directory, looks for info.csv, myimage.jpg (converts to base 64), reports on files. 
            foreach (var directory in directories)
            {
                Student student = new Student() { AbsoluteUrl = Constants.FTP.BaseUrl };
                student.FromDirectory(directory);
                string infofilepath = student.FullPathUrl + "/" + Constants.Locations.InfoFile;
                string imagefilepath = student.FullPathUrl + "/" + Constants.Locations.ImageFile;
                bool InfofileExists = FTP.FileExists(infofilepath);
                bool ImagefileExists = FTP.FileExists(imagefilepath);

                if (InfofileExists == true)
                {
                    Console.WriteLine(student);
                    string csvpath = $@"C:\Users\rober\Documents\BDAT1000_Working\\{directory}.csv";
                    byte[] bytes = FTP.DownloadFileBytes(infofilepath);
                    ///Downloads the csv files
                    /// FTP.DownloadFile(infofilepath, csvpath);
                    string csvData = Encoding.Default.GetString(bytes);

                    string[] csvlines = csvData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                    if (csvlines.Length != 2)
                    {
                        Console.WriteLine("Error in CSV format.");
                    }
                    else
                    {
                        student.FromCSV(csvlines[1]);
                    }
                    Console.WriteLine("Found Info File");
                    Console.WriteLine(infofilepath);
                }
                else
                {
                    Console.WriteLine("Could not find info file:");
                }
                if (ImagefileExists == true)
                {
                    Console.WriteLine("Found Image File");
                    Console.WriteLine(imagefilepath + "\n");

                }
                else
                {
                    Console.WriteLine("Could not find image file:");
                }
                ///ends the section
                students.Add(student);
            }
            //save to CSV file
            //Answer 5.2
            // Establishes stream, creates csv file, builds a new object capable of being converted to JSON/XML

            List<JsonStudent> JsonStudents = new List<JsonStudent>();
            using (StreamWriter fs = new StreamWriter(Models.Constants.Locations.studentsCSVPath))
            {
                fs.WriteLine((nameof(Student.StudentId)) +','+ (nameof(Student.FirstName)) +','+ (nameof(Student.LastName)) + ',' + (nameof(Student.Age)) + ',' + (nameof(Student.DateOfBirth)) + ',' + (nameof(Student.MyRecord)) + ',' + (nameof(Student.Image)));
                foreach (var student in students)
                {
                    fs.WriteLine(student.ToCSV());                                       
                    JsonStudent jsonstudent = new JsonStudent();
                    jsonstudent.FromStudent(student);                   
                    JsonStudents.Add(jsonstudent);
                }
            }
           
            /////converts to JSON and saves a JSON file                      
            string jsonconvert = System.Text.Json.JsonSerializer.Serialize(JsonStudents);
            File.WriteAllText(Models.Constants.Locations.studentsJSONPath, jsonconvert);
            
            ///creates XML root, converts JSON list to XML, Saves XML
                var xEle = new XElement("Students",
                    from student in JsonStudents
                    select new XElement("Student",                        
                           new XAttribute("StudentID", student.StudentId),
                                        new XElement("First_Name", student.FirstName),
                                        new XElement("Last_Name", student.LastName),
                                        new XElement("Age", student.Age),
                                        new XElement("Date_of_Birth", student.DateOfBirth),
                                        new XElement("My_Record", student.MyRecord),
                                        new XElement("Image", student.Image)
                        ));
                xEle.Save(Constants.Locations.studentsXMLPath);              
              

            //creates variables to answer questions about student list.
            List<int> ages = new List<int>();
            List<string> startswc = new List<string>();
            List<string> containsc = new List<string>();
            int TotalAge = 0;
            int MaxAge = 0;
            int MinAge = 0;
            int validages = 0;

            //Printing names using ToCSV and ToString
            Console.WriteLine("The student names and information are, using ToCSV:");
            foreach (var student in students)
                {
                    Console.WriteLine(student.ToCSV());                
                }
            Console.WriteLine("The student names and information are, using ToString:");
            foreach (var student in students)
                {
                    Console.WriteLine(student.ToString());
                }

            //Getting StartsWith, Contain and Average/Max/Min
            foreach (var student in students)
            {
                if (student.LastName.StartsWith("C"))
                {
                    startswc.Add(student.LastName);
                }
                if (student.LastName.StartsWith("c"))
                {
                    startswc.Add(student.LastName);
                }
                bool a = student.LastName.Contains("c");
                bool b = student.LastName.Contains("C");
                if (a || b)
                {
                    containsc.Add(student.LastName);
                }

                ages.Add(student.Age);               

            }
            //reducing ages entered in spreadsheets to only the usable values
            foreach (var age in ages)
            {
                if (age < 1)
                {
                    continue;
                }
                else if (age > 100)
                {
                    continue;
                }
                else
                {
                    TotalAge = TotalAge + age;
                    if (age > MaxAge)
                    {
                        MaxAge = age;
                    }

                    if (MinAge == 0)
                    {
                        MinAge = age;
                    }
                    else if (age < MinAge);
                    {
                        MinAge = age;
                    }
                    validages = validages + 1;
                }
            }
        ///writing answers to Console
        Console.WriteLine("\nThere are " + students.Count + " entries in the student list");
        var Answer51b = "\n" + startswc.Count + " students have last names starting with C. The last names are: \n";
        var Answer51c = "\n" + containsc.Count + " students have last names containing 'c' or 'C'. The last names are: \n";
        var Answer51e = "\nThe Average Age of the students who entered a valid age is " + (TotalAge/validages);
        var Answer51f = "\nThe highest age is " + MaxAge;
        var Answer41g = "\nThe lowest age is " + MinAge;
        Console.Write(Answer51b);
        foreach (var i in startswc)
        {
            Console.Write(i + "  \n");
        }
        Console.Write(Answer51c);
        foreach (var i in containsc)
        {
            Console.Write(i + "  \n");
        }
        Console.WriteLine(Answer51e);
        Console.WriteLine(Answer51f);
        Console.WriteLine(Answer41g);

            ///uploads files to ftp site
            FTPUpload.UploadFile(Constants.Locations.studentsCSVPath,Constants.FTP.remoteUploadFileDestinationcsv);
            FTPUpload.UploadFile(Constants.Locations.studentsJSONPath, Constants.FTP.remoteUploadFileDestinationjson);
            FTPUpload.UploadFile(Constants.Locations.studentsXMLPath, Constants.FTP.remoteUploadFileDestinationxml);
            return;
              
        }
    }
}
