using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DemoProject.Models;
using System.Data;
using Dapper;

namespace DemoProject.Repository
{
    public class EmpRepository
    {
        public SqlConnection con;
        //To Handle connection related activities
        private void Connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ToString();
            con = new SqlConnection(constr);
        }
        //To Add Employee details
        public void AddEmployee(EmployeeModel objEmp)
        {
            //Additing the employess
            try
            {
                int id = RandomNumber(1, 500);
                Connection();
                con.Open();
                con.Execute("AddNewDemoproj", objEmp, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Generate a random number between two numbers
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        //To view employee details with generic list 
        public List<EmployeeModel> GetAllEmployees()
        {
            try
            {
                Connection();
                con.Open();
                IList<EmployeeModel> EmpList = SqlMapper.Query<EmployeeModel>(
                                  con, "GetDemoprojDetails").ToList();
                con.Close();
                return EmpList.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //To Update Employee details
        public void UpdateEmployee(EmployeeModel objUpdate)
        {
            try
            {
                Connection();
                con.Open();
                con.Execute("UpdateEmpDetails", objUpdate, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //To delete Employee details
        public bool DeleteEmployee(int Id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                Connection();
                con.Open();
                con.Execute("DeleteDemoprojById", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                //Log error as per your need 
                throw ex;
            }
        }
        public int GetImageCount(string filename)
        {
            DynamicParameters ObjParm = new DynamicParameters();
            ObjParm.Add("@FileName", filename);
            ObjParm.Add("@ImageCount", dbType: DbType.Int16, direction: ParameterDirection.Output, size: 5215585);
            Connection();
            con.Open();
            con.Execute("GetImageCount", ObjParm, commandType: CommandType.StoredProcedure);
            //Getting the out parameter value of stored procedure  
            var Count = ObjParm.Get<Int16>("@ImageCount");
            con.Close();
            return Count;

        }
        //To view employee details with generic list 
        public List<EmployeeDetailsModel> GetAllEmpList()
        {
            try
            {
                Connection();
                con.Open();
                IList<EmployeeDetailsModel> EmpList = SqlMapper.Query<EmployeeDetailsModel>(
                                  con, "select T1.EmpId,T1.Name,T1.Location,T1.Subject,T1.Grade,T2.Status from tbl2EmpDetails T1 left join tbl2DailyStatus T2 on T1.EmpId=T2.EmpId and T2.Date=(SELECT CONVERT(VARCHAR(10), GETDATE(), 103) AS [DD/MM/YYYY])").ToList();
                con.Close();
                return EmpList.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //To update Employee status
        public bool UpdateEmployeeStatus(int Id,string Location,String ActionParam)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                param.Add("@Location", Location);
                param.Add("@ActionParam", ActionParam);
                Connection();
                con.Open();
                con.Execute("sp2UpdateEmployeeStatusById", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                //Log error as per your need 
                throw ex;
            }
        }
        //To view employee details with generic list 
        public List<EmployeeDailyStatusModel> GetEmpStatusDetailsByID(int Id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                Connection();
                con.Open();
                IList<EmployeeDailyStatusModel> EmpList = SqlMapper.Query<EmployeeDailyStatusModel>(
                                  con, "select * from tbl2DailyStatus where EmpId="+ Id).ToList();
                con.Close();
                return EmpList.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}