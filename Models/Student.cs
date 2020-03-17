using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Routledge_Assignment_3.Models
{
    public class Student
    {
        public static string HeaderRow = "{nameof(Student.StudentId)},{nameof(Student.FirstName)},{nameof(Student.LastName)},{nameof(Student.Age},{nameof(Student.DateOfBirth)},nameof(Student.ImageData)}";

        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MyRecord { get; set; }


        private string _DateOfBirth;
        public string DateOfBirth
        {
            get { return _DateOfBirth; }
            set
            {
                _DateOfBirth = value;

                //Convert DateOfBirth to DateTime
                DateTime dtOut;
                DateTime.TryParse(_DateOfBirth, out dtOut);
                DateOfBirthDT = dtOut;
            }
        }
        public DateTime DateOfBirthDT { get; internal set; }

        public string Image { get; set; }

        public string AbsoluteUrl { get; set; }

        public string FullPathUrl
        {
            get
            {
                return AbsoluteUrl + "/" + Directory;
            }
        }
        public string Directory { get; set; }

        public List<string> Exceptions { get; set; } = new List<string>();

        public void FromCSV(string csvdata)
        {
            string[] data = csvdata.Split(",", StringSplitOptions.None);
            try
            {
                StudentId = data[0];
                FirstName = data[1];
                LastName = data[2];
                DateOfBirth = data[3];
                Image = data[4];
            }
            catch (Exception e)
            {
                Exceptions.Add(e.Message);
            }

        }

        /// Because there is no set, this is a read only variable   
        public int Age { get { return (DateTime.Today.Year - DateOfBirthDT.Year); } }

        public void FromDirectory(string directory)
        {
            Directory = directory;

            if (String.IsNullOrEmpty(directory.Trim()))
            {
                return;
            }

            string[] data = directory.Trim().Split(" ", StringSplitOptions.None);

            StudentId = data[0];
            FirstName = data[1];
            LastName = data[2];
        }


        public string ToCSV()
        {
            ///Answers 5.1.d
            if (StudentId == "200449068")
            {
                MyRecord = "you";
            }

            string result = $"{StudentId},{FirstName},{LastName},{Age},{DateOfBirth},{MyRecord},{Image}";
            
            return result;
        }

        public override string ToString()
        {
            string result = $"{StudentId} {FirstName} {LastName}";
            return result;
        }
        
       
        
     



    }
    public class JsonStudent
    {
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MyRecord { get; set; }
        public string Image { get; set; }

        public void FromStudent (Student sample)
        {
            StudentId = sample.StudentId;
            FirstName = sample.FirstName;
            LastName = sample.LastName;
            DateOfBirth = sample.DateOfBirthDT;
            Age = sample.Age;
            MyRecord = sample.MyRecord;
            Image = sample.Image;
        }
       
    }
   
}
