using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPM.Helpers
{
    public class Calculations
    {
        public static double sumTestGrades(List<Entities.Test> testtList)
        {
            double userGrade = 0;
            for (int i = 0; i < testtList.Count; i++)
            {
                if (testtList[i].grade.HasValue)
                {
                    userGrade += (double)testtList[i].grade;
                }
            }

            return userGrade;           
        }

        public static double sumProjectGrades(List<Entities.Project> projectList)
        {
            double userGrade = 0;
            for (int i = 0; i < projectList.Count; i++)
            {
                if (projectList[i].grade.HasValue)
                {
                    userGrade += (double)projectList[i].grade;
                }
            }

            return userGrade;
        }

        public static double calculateClassGradePercent(List<Entities.Project> projectList, List<Entities.Test> testtList)
        {
            int sumProjectOutOf = 0;
            double sumTestOutOf = 0;
            double sumGrades = Calculations.sumProjectGrades(projectList) + Calculations.sumTestGrades(testtList);
            

            for (int i = 0; i < projectList.Count; i++)
            {
                if (projectList[i].grade_out_of != 0)
                {
                    sumProjectOutOf += projectList[i].grade_out_of;
                }
            }

            for (int i = 0; i < testtList.Count; i++)
            {
                if (testtList[i].grade_out_of != 0)
                {
                    sumTestOutOf += testtList[i].grade_out_of;
                }
            }
            double calculation = Math.Round((sumGrades / (sumProjectOutOf + sumTestOutOf)) * 100, 2);

            if(calculation > 0)
            {
                return calculation;
            }
            else
            {
                return 0;
            }         
        }
    }
}