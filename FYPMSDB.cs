using System.Data;
using Oracle.DataAccess.Client;


namespace FYPMSWebsite.App_Code
{
    /// <summary>
    /// Student name: Emre Uludag       
    /// Student number: 20662309
    /// 
    /// NOTE: This is an individual task. By submitting this file you certify that this
    /// code is the result of your individul effort and that it has not been developed
    /// in collaoration with or copied from any other person. If this is not the case,
    /// then you must identify by name all the persons with whom you collaborated or
    /// from whom you copied code below.
    /// 
    /// Collaborators: 
    /// </summary>

    public class FYPMSDB
    {
        //******************************** IMPORTANT NOTE **********************************
        // For the web pages to display a query result correctly, the attribute names in   *
        // the query result table must be EXACTLY the same as that in the database tables. *
        //         Report problems with the website code to 3311rep@cse.ust.hk.            *
        //**********************************************************************************

        private OracleDBAccess myOracleDBAccess = new OracleDBAccess();
        private string sql;

        public DataTable GetProjectsWithoutReaders()
        {
            //**************************************************************************
            // TODO 01: Used in Coordinator/AssignReader.aspx.cs                       *
            // Construct the SQL SELECT statement to retrieve the group id, group code *
            // and project title, category and type for the project groups that DO NOT *
            // have an assigned reader. Order the result by group code ascending.      *
            //**************************************************************************
            sql = "SELECT P.groupId,P.groupCode,P.fypAssigned,F.title,F.fypCategory,F.fypType FROM ProjectGroup P ,FYProject F WHERE P.fypAssigned = F.fypId and P.reader is null order by P.groupCode asc";
            return myOracleDBAccess.GetData(sql);
        }

        public decimal NumberProjectsAssignedToReader(string username)
        {
            //*****************************************************************************
            // TODO 02: Used in Coordinator/AssignReader.aspx.cs                          *
            // Construct the SQL SELECT statement to retrieve the the number of projects  *
            // to which a faculty, identified by his/her username, is assigned as reader. *
            //*****************************************************************************
            sql = "SELECT count(*) FROM Faculty left outer join ProjectGroup on reader = username where projectGroup.reader = '" + username + "' group by faculty.username";
            return myOracleDBAccess.GetAggregateValue(sql);
        }

        public bool AssignReaderToProject(string groupId, string username)
        {
            //**********************************************************************
            // TODO 03: Used in AssignReader.aspx.cs                               *
            // Construct the SQL UPDATE statement to assign a reader, identified   * 
            // by his/her username, to a project group, identified by its groupId. *
            //**********************************************************************
            sql = "UPDATE ProjectGroup SET reader = '" + username + "' WHERE ProjectGroup.groupId = " + groupId;
            return SetData(sql);
        }

        public DataTable GetAssignedReaders()
        {
            //************************************************************************
            // TODO 04: Used in Coordinator/DisplayProjectReaders.aspx.cs            *
            // Construct the SQL SELECT statement to retrieve the reader name, group * 
            // code and project title for the project groups with assigned readers.  *
            // Order the result by group code ascending.                             *
            //************************************************************************
            sql = "SELECT Faculty.facultyName,ProjectGroup.groupCode,FYProject.title FROM ProjectGroup,FYProject,Faculty WHERE ProjectGroup.fypAssigned = FYProject.fypId and Faculty.username = ProjectGroup.reader and ProjectGroup.reader is not null order by projectgroup.groupcode asc";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetFacultyCode(string username)
        {
            //*************************************************************
            // TODO 05: Used in Faculty/AssignGrades.aspx.cs              *
            // Construct the SQL SELECT statement to retrieve the faculty *+++
            // code of a faculty identified by his/her username.          *
            //*************************************************************
            sql = "SELECT facultyCode from faculty where faculty.username = '" + username + "'";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetFacultyGroups(string facultyCode)
        {
            //****************************************************************************************
            // TODO 06: Used in Faculty/AssignGrades.aspx.cs                                         *
            // Construct the SQL SELECT statement to retrieve the group id, group code and FYP id of *
            // the project to which the group is assigned for the groups supervised by the faculty   *+++
            // identified by his/her faculty code. Order the result by group id ascending.           *
            //****************************************************************************************
            sql = "SELECT P.groupId,P.groupCode,P.fypAssigned FROM PROJECTGROUP P,FACULTY F,SUPERVISES S WHERE (S.fypId = p.fypassigned and S.username = F.username) and f.facultycode = '" + facultyCode + "' ORDER BY P.groupId asc";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetStudentRequirements(string groupId, string fypId)
        {
            //****************************************************************************************
            // TODO 07: Used in Faculty/AssignGrades.aspx.cs                                         *
            // Construct the SQL SELECT statement to retrieve students' username, name and all the   *
            // requirement grades given by any of the faculty that are the supervisors of a project  *+++
            // identified by its fyp id, for all the students in a group identified by its group id. *
            //****************************************************************************************
            sql = "select s.username,s.studentname,r.proposalgrade,r.progressgrade,r.finalgrade,r.presentationgrade from requirement R,Students S,Projectgroup G,Faculty F where R.studentusername = S.username and F.username = R.facultyUsername and s.groupid = g.groupId and s.groupid = " + groupId + " and g.fypAssigned = " + fypId;
            return myOracleDBAccess.GetData(sql);
        }

        public bool UpdateGrades(string fypId, string studentUsername, string proposalGrade,
            string progressGrade, string finalGrade, string presentationGrade)
        {
            //***************************************************************************************
            // TODO 08: Used in Faculty/AssignGrades.aspx.cs                                        *
            // Construct the SQL UPDATE statement to update ALL grade values in a requirement       *
            // record, identified by a faculty username and a student username. NOTE: While a grade *
            // can be updated by ANY of the supervisors of a project, only the username of the      *
            // faculty who assigned a group to a project appears in the Requirement table record.   *                                                  *
            //***************************************************************************************
            sql = "update Requirement set proposalGrade=" + proposalGrade + ",progressGrade=" + progressGrade + ",finalGrade=" + finalGrade + ",presentationGrade=" + presentationGrade + " where studentUsername='" + studentUsername + "' and facultyUsername in (select S.username from Supervises S where s.fypid = " + fypId + ")";
            return SetData(sql);
        }

        public DataTable GetProjectFacultyCodes(string fypId)
        {
            //******************************************************************
            // TODO 09: Used in Faculty/AssignGroupToProject.aspx.cs           *
            // Construct the SQL SELECT statement to retrieve the faculty      *+++
            // codes of the supervisors of a project identified by its FYP id. *
            // Order the result by faculty code ascending.                     *
            //******************************************************************
            sql = "SELECT F.facultyCode FROM FACULTY F ,FYProject P,Supervises S WHERE P.fypId = " + fypId + " and S.username=F.username and S.fypId = P.fypId ORDER BY F.facultyCode asc";
            return myOracleDBAccess.GetData(sql);
        }

        public decimal GetFacultyCodeSequenceNumber(string groupCodePrefix)
        {
            //************************************************************
            // TODO 10: Used in Faculty/AssignGroupToProject.aspx.cs     *
            // Construct the SQL SELECT statement to retrieve the number *
            // of times a given group code prefix has been used.         *
            // A group code prefix is the group code minus its trailing  *
            // integer (e.g., for group code "FL1" the prefix is "FL").  *
            //************************************************************
            sql = "select count(*) from projectGroup where substr(groupCode, 1,2) = '" + groupCodePrefix + "' group by substr(groupCode, 1,2)";
            return myOracleDBAccess.GetAggregateValue(sql);
        }

        public DataTable GetGroupsAvailableToAssign(string fypId)
        {
            //********************************************************************************
            // TODO 11: Used in Faculty/AssignGroupToProject.aspx.cs                         *
            // Construct the SQL SELECT statement to retrieve the group id and priority, as  *
            // well as the name and username of the students in the group, for those groups  *
            // that are available for assignment and that have have indicated an interest in *
            // a project identified by its fyp id where the project is available. Order the  *
            // result first by group id ascending and then by student name ascending.        *
            //********************************************************************************
            sql = "select groupId,fyppriority,studentname,username from ProjectGroup natural join InterestedIn natural join Students where fypId = "+fypId+" and fypAssigned is null order by groupId asc,studentname asc";
            //sql = "select s.studentname,s.username,i.groupId,i.fyppriority from interestedin i,students s where i.groupid = s.groupid and i.fypId = " + fypId + " order by i.groupid asc, s.studentname asc";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetGroupsCurrentlyAssigned(string fypId)
        {
            //**********************************************************************************
            // TODO 12: Used in Faculty/AssignGroupToProject.aspx.cs                           *
            // Construct the SQL SELECT statement to retrieve the group id and group code, as  *
            // well as the name of the students in the group, for those groups that have been  *+++
            // assigned to a project identified by its fyp id. Order the result first by group *
            // id ascending and then by student name ascending.                                *
            //**********************************************************************************
            sql = "select s.studentName,g.groupId,g.groupCode from ProjectGroup G,Students S where s.groupId = G.groupId and g.fypassigned = " + fypId + " order by g.groupId asc,s.studentname desc ";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetFacultyProjects(string username)
        {
            //************************************************************
            // TODO 13: Used in Faculty/AssignGroupToProject.aspx.cs     *
            // Construct the SQL SELECT statement to retrieve the id and *
            // title of the projects supervised by a faculty identified  *+++
            // by his/her username. Order the result by title ascending. *
            //************************************************************
            sql = "SELECT FYProject.fypId,FYProject.title FROM FYProject,Supervises WHERE Supervises.fypId =FYProject.fypID and Supervises.username = '" + username + "'";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetProjectAvailability(string fypId)
        {
            //********************************************************
            // TODO 14: Used in Faculty/AssignGroupToProject.aspx.cs *
            // Construct the SQL SELECT statement to retrieve the    *+++
            // availability of a project identified by its fyp id.   *
            //********************************************************
            sql = "SELECT isAvailable from FYPROJECT where fypid = " + fypId;
            return myOracleDBAccess.GetData(sql);
        }

        public bool AssignGroupToProject(string groupCode, string fypId, string groupId)
        {
            //*****************************************************************************
            // TODO 15: Used in Faculty/AssignGroupToProject.aspx.cs                      *
            // Construct the SQL UPDATE statement to assign a project group to a project. *+++
            //*****************************************************************************
            sql = "UPDATE ProjectGroup SET fypAssigned= " + fypId + " WHERE groupId=" + groupId + " or groupCode= '" + groupCode + "'";
            return SetData(sql);
        }

        public decimal GetNumberOfGroupsSupervised(string username)
        {
            //***********************************************************************************
            // TODO 16: Used in Faculty/AssignGroupToProject.aspx.cs                            *
            // Construct the SQL SELECT statement to retrieve the total number of groups that a * +++
            // faculty, identified by his/her username, has assigned to all of his/her projects.*
            //***********************************************************************************
            sql = "SELECT COUNT(*) FROM Supervises where username = '" + username + "' GROUP BY username";
            return myOracleDBAccess.GetAggregateValue(sql);
        }

        public bool CreateFYProject(string fypId, string title, string fypDescription, string fypCategory,
            string fypType, string requirement, string minStudents, string maxStudents, string isAvailable,
            string supervisor, string cosupervisor)
        {
            // First, create an Oracle transaction.
            OracleTransaction trans = myOracleDBAccess.BeginTransaction();
            if (trans == null) { return false; }

            // Second, create the project record.
            //*******************************************************************
            // TODO 17: Used in Faculty/CreateProject.aspx.cs                   *+++
            // Construct the SQL INSERT statement to insert a FYProject record. *
            //*******************************************************************
            sql = "insert into FYProject values (" + fypId + ",'" + title + "','" + fypDescription + "','" + fypCategory + "','" + fypType + "','" + requirement + "'," + minStudents + "," + maxStudents + ",'" + isAvailable + "')";
            if (!myOracleDBAccess.SetData(sql, trans)) { myOracleDBAccess.DisposeTransaction(trans); return false; }

            // Third, create the Supervises record for the supervisor.
            //***************
            // Uses TODO 18 *
            //***************
            if (!CreateSupervises(supervisor, fypId, trans)) { myOracleDBAccess.DisposeTransaction(trans); return false; }
            {
                // Create the Supervises record for the cosupervisor, if any.
                if (cosupervisor != "")
                {
                    if (!CreateSupervises(cosupervisor, fypId, trans)) { myOracleDBAccess.DisposeTransaction(trans); return false; }
                }
            }
            myOracleDBAccess.CommitTransaction(trans);
            return true;
        }

        public bool CreateSupervises(string username, string fypId, OracleTransaction trans)
        {
            if (trans == null)
            {
                trans = myOracleDBAccess.BeginTransaction();
                if (trans == null) { return false; }
            }
            //********************************************************************
            // TODO 18: Used in App_Code/FYPMSDB.CreateFYProject                 *+++
            // Construct the SQL INSERT statement to insert a Supervises record. *
            //********************************************************************
            sql = "insert into Supervises values ('" + username + "'," + fypId + ")";
            return myOracleDBAccess.SetData(sql, trans);
        }

        public DataTable GetSupervisorProjectDigest(string username)
        {
            //*******************************************************************************
            // TODO 19: Used in Faculty/DisplayProjects.aspx.cs                             *
            // Construct the SQL SELECT statement to retrieve the fyp id, title, category,  *
            // type, mimimum students and maximum students of the projects supervised by a  *+++
            // faculty identified by his/her username. Order the result by title ascending. *
            //*******************************************************************************
            sql = "SELECT fyproject.fypId,fyproject.title,fypcategory,fyptype,fyproject.minstudents,fyproject.maxstudents from FYProject,Supervises where supervises.fypid = fyproject.fypid and supervises.username = '" + username + "' order by fyproject.title asc";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetInterestedInProject(string fypId)
        {
            //**********************************************************************
            // TODO 20: Used in Faculty/EditProject.aspx.cs                        *
            // Construct the SQL SELECT statement to retrieve all the attributes   * +++
            // from the InterestedIn table for a project identified by its fyp id. *
            //**********************************************************************
            sql = "select * from interestedin where fypid = " + fypId;
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetCosupervisorInfoForEdit(string fypId, string username)
        {
            //**************************************************************************************
            // TODO 21: Used in Faculty/EditProject.aspx.cs                                        *
            // Construct the SQL SELECT statement to retrieve the username of the cosupervisor, if *
            // any, of a project, identified by its fyp id, given the username of one supervisor.  *
            //**************************************************************************************
            sql = "select username from Supervises where Supervises.fypId in (select fypId from Supervises group by fypId having count(*)> 1) and fypid = " + fypId + " and username <> '" + username + "'";
            return myOracleDBAccess.GetData(sql);
        }

        public bool UpdateFYProject(string fypId, string title, string fypDescription, string fypCategory,
            string fypType, string requirement, string minStudents, string maxStudents, string isAvailable,
            string oldCosupervisor, string newCosupervisor)
        {
            // First, create an Oracle transaction.
            OracleTransaction trans = myOracleDBAccess.BeginTransaction();
            if (trans == null) { return false; }

            // Second, update the FYP project values.
            //***********************************************************************************
            // TODO 22: Used in Faculty/EditProject.aspx.cs                                     *
            // Construct the SQL UPDATE statement to update all the values of a FYProject table *
            // record with the corresponding parameter value for the given project fyp id.      *
            //***********************************************************************************
            sql = "UPDATE FYProject SET fypId=" + fypId + ",title='" + title + "',fypDescription='" + fypDescription + "',fypCategory='" + fypCategory + "',fypType='" + fypType + "',requirement='" + requirement + "',minStudents=" + minStudents + ",maxStudents=" + maxStudents + ",isAvailable='" + isAvailable + "' WHERE fypId=" + fypId;
            if (!myOracleDBAccess.SetData(sql, trans)) { myOracleDBAccess.DisposeTransaction(trans); return false; }

            // Finally, update the cosupervisor, if necessary.
            if (oldCosupervisor != newCosupervisor)
            {
                if (oldCosupervisor != "")
                {
                    // Delete the old cosupervsior for the project from the Supervises table.
                    //***************
                    // Uses TODO 23 *
                    //***************
                    if (!DeleteSupervises(oldCosupervisor, fypId, trans)) { myOracleDBAccess.DisposeTransaction(trans); return false; }
                }
                if (newCosupervisor != "")
                {
                    // Insert a new cosupervisor for the project into the Supervises table.
                    //***************
                    // Uses TODO 18 *
                    //***************
                    if (!CreateSupervises(newCosupervisor, fypId, trans)) { myOracleDBAccess.DisposeTransaction(trans); return false; }
                }
            }
            myOracleDBAccess.CommitTransaction(trans);
            return true;
        }

        public bool DeleteSupervises(string username, string fypId, OracleTransaction trans)
        {
            //**************************************************************
            // TODO 23: Used in App_Code/FYPMSDB.UpdateFYProject           *
            // Construct the SQL DELETE statement to delete the Supervises *+++
            // record identified by an fyp id and a username.              *
            //**************************************************************
            sql = "DELETE FROM SUPERVISES WHERE fypid=" + fypId + " and username= '" + username + "'";
            return myOracleDBAccess.SetData(sql, trans);
        }

        public bool CreateInterestedIn(string fypId, string groupId, string fypPriority)
        {
            //*************************************************************************
            // TODO 24: Used in Student/AvailableProjects.aspx.cs                     *
            // Construct the SQL INSERT statement to add a record to the InterestedIn *+++
            // table with the specified fyp id, group id and priority values.         *
            //*************************************************************************
            sql = "insert into InterestedIn values (" + fypId + ", " + groupId + ", " + fypPriority + ")";
            return SetData(sql);
        }

        public DataTable GetProjectAssignedToGroup(string groupId)
        {
            //***************************************************************
            // TODO 25: Used in Student/AvailableProjects.aspx.cs           *
            // Construct the SQL SELECT statement to retrieve the title of  *
            // the project assigned to the group identified by its groupId. *+++
            // Order the result by title ascending.                         * 
            //***************************************************************
            sql = "select f.title from fyproject f,projectgroup p where f.fypid=p.fypassigned and p.groupid =" + groupId + "order by f.title asc";
            return myOracleDBAccess.GetData(sql);
        }

        public bool AddStudentToGroup(string groupId, string username)
        {
            //***********************************************************************
            // TODO 26: Used in Student/ManageProjectGroup.aspx.cs                  *
            // Construct the SQL UPDATE statement to assign a groupId to a student. * +++
            //***********************************************************************
            sql = "update Students set groupId=" + groupId + "where username='" + username + "'";
            return SetData(sql);
        }

        public DataTable GetStudentRecord(string username)
        {
            //****************************************************************
            // TODO 27: Used in Student/ManageProjectGroup.aspx.cs           *
            // Construct the SQL SELECT statement to retrieve all of the     *
            // attribute values of a student identified by his/her username. *
            //****************************************************************
            sql = "select * from Students where username = '" + username + "'";
            return myOracleDBAccess.GetData(sql);
        }

        public bool RemoveGroupMember(string username)
        {
            //*********************************************************
            // TODO 28: Used in Student/ManageProjectGroup.aspx.cs    *
            // Construct the SQL UPDATE statement to remove a student *+++
            // identified by his/her username from a project group.   *
            //*********************************************************
            sql = "update Students set groupId= null where username='" + username + "'";
            return SetData(sql);
        }

        public bool DeleteProjectGroup(string groupId)
        {
            //******************************************************
            // TODO 29: Used in Student/ManageProjectGroup.aspx.cs *
            // Construct the SQL DELETE statement to delete a      *
            // project group identified by its groupId.            *
            //******************************************************
            sql = "DELETE FROM ProjectGroup where groupId = " + groupId;
            return SetData(sql);
        }

        public bool CreateProjectGroup(string groupId)
        {
            //*********************************************************************************
            // TODO 30: Used in Student/ManageProjectGroup.aspx.cs                            *
            // Construct the SQL INSERT statement to add a project group with the specified   *+++
            // groupId to the ProjectGroup table. All other attributes values should be null. *
            //*********************************************************************************
            sql = "insert into ProjectGroup values (" + groupId + ",null,null,null)";
            return SetData(sql);
        }

        public DataTable GetAssignedProjectInformatione(string username)
        {
            //*****************************************************************************
            // TODO 31: Used in Student/ViewGrades.aspx.cs                                *
            // Construct the SQL SELECT statement to retrieve the fyp id and title of the * +++
            // project to which a student, identified by his/her username, is assigned.   *
            //*****************************************************************************
            sql = "select F.fypId,F.title from FYProject F,projectgroup P,students S where s.groupid = p.groupid and p.fypassigned = f.fypid and s.username = '" + username + "'";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetStudentGrades(string username)
        {
            //****************************************************************************
            // TODO 32: Used in Student/ViewGrades.aspx.cs                               *
            // Construct the SQL SELECT statement to retrieve the faculty name, as well  *
            // as the proposal, progress, final and presentation grades, given by the    *+++
            // supervisor and the reader for the student identified by his/her username. *
            //****************************************************************************
            sql = "select f.facultyname,r.proposalgrade,r.progressgrade,r.finalgrade,r.presentationgrade from Faculty F,Requirement R,students S where s.username = r.studentusername and f.username = r.facultyusername and s.username = '" + username + "'";
            return myOracleDBAccess.GetData(sql);
        }

        /***********************************************/
        /***** Methods used in App_Code/Helpers.cs *****/
        /***********************************************/

        public bool CreateRequirement(string facultyUsername, string studentUsername, string proposalGrade,
            string progressGrade, string finalGrade, string presentationGrade)
        {
            //******************************************************************
            // TODO 33: Used in CreateRequirementRecord in App_Code/Helpers.cs *
            // Construct the SQL INSERT statement to insert a value for each   *
            // attribute of the Requirement table whose values are specified   *+++
            // by the corresponding parameter of this method.                  *
            //******************************************************************
            sql = "insert into Requirement values ('" + facultyUsername + "','" + studentUsername + "'," + proposalGrade + "," + progressGrade + "," + finalGrade + "," + presentationGrade + ")";
            return SetData(sql);
        }

        public DataTable GetAssignedFypId(string groupId)
        {
            //***************************************************************************
            // TODO 34: Used in IsGroupAssigned in App_Code/Helpers.cs                  *
            // Construct the SQL SELECT statement to retrieve the fyp id of the project * +++
            // to which a project group, identified by its group id, has been assigned. *
            //***************************************************************************
            sql = "select P.fypAssigned from ProjectGroup P where p.groupid = " + groupId;
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetFaculty()
        {
            //*****************************************************
            // TODO 35: Used in GetFaculty in App_Code/Helpers.cs *
            // Construct the SQL SELECT statement to retrieve     * +++
            // the username and name of all faculty.              *
            //*****************************************************
            sql = "select username,facultyName from faculty";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetGroupAvailableProjectDigests(string groupId)
        {
            //**************************************************************************************
            // TODO 36: Used in GetGroupAvailableProjectDigests in App_Code/Helpers.cs             *
            // Construct the SQL SELECT statement to retrieve the fyp id, title, category, type,   *
            // minimum students and maximum students of the projects for which a group, identified *
            // by its group id, can indicate an interest. Only those projects that are available,  *+++
            // meet the group's size requirements and for which a group has not alredy indicated   *
            // an interest should be retrieved. Groups that have been assigned to a project cannot *
            // indicate an interest in any project. Order the result by title ascending.           *                                       *
            //**************************************************************************************
            sql = "with countGroup(groupNum,numberOfPartic) as (select groupId,count(*) from Students group by groupId), notinterested(projectNum) as (select F.fypId from FYProject f where f.fypId not in (select fypId from InterestedIn)) select distinct F.fypId,F.title,F.fypCategory,F.fypType,F.minStudents,F.maxStudents from FYProject F,ProjectGroup G where F.fypId in (select * from notinterested) and g.groupId = " + groupId + " and g.fypAssigned is null and  (F.minStudents <= (select numberOfPartic from countGroup where groupNum = " + groupId + " )and F.maxStudents >= (select numberOfPartic from countGroup where groupNum = " + groupId + " ) ) order by F.title";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetProjectGroupMembers(string groupId)
        {
            //****************************************************************************
            // TODO 37: Used in GetProjectGroupMembers in App_Code/Helpers.cs            *
            // Construct the SQL SELECT statement to retrieve the value of all the       * +++
            // attributes of the students in a project group identified by its group id. *
            //****************************************************************************
            sql = "select s.username,s.studentname,s.groupid from Students S,projectgroup p where s.groupid = p.groupid and p.groupid = " + groupId;
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetStudentGroupId(string username)
        {
            //************************************************************
            // TODO 38: Used in GetStudentGroupId in App_Code/Helpers.cs *
            // Construct the SQL SELECT statement to retrieve the        * +++  
            // group id for the student identified by his/her username.  *
            //************************************************************
            sql = "select groupId from Students where username = '" + username + "'";
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetSupervisors(string fypId)
        {
            //**************************************************************************
            // TODO 39: Used in GetProjectSupervisors in App_Code/Helpers.cs           *
            // Construct the SQL SELECT statement to retrieve the username and faculty *+++
            // name of all the supervisors of a project identified by its fyp id.      *
            //**************************************************************************
            sql = "SELECT F.username,F.facultyName from Faculty F ,Supervises S where S.username = F.username and S.fypId =" + fypId;
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetProjectsGroupInterestedIn(string groupId)
        {
            //*********************************************************************************
            // TODO 40: Used in GetProjectsGroupInterestedIn in App_Code/Helpers.cs           *
            // Construct the SQL SELECT statement to retrieve the fyp id, title, category,    *+++
            // type and priority of the projects for which a group, identified by its group   *
            // id, HAS ALREADY indicated an interest. Order the result by priority ascending. *
            //*********************************************************************************
            sql = "select p.fypId,p.title,p.fypcategory,p.fyptype,i.fyppriority from FYProject P,interestedin I where i.fypid = p.fypid and I.groupid = " + groupId + " order by i.fyppriority asc";
            return myOracleDBAccess.GetData(sql);
        }

        /*---------------------------------END OF TODOS---------------------------------*/

        #region *** DO NOT CHANGE THE METHODS BELOW THIS LINE. THEY ARE NOT TODOS!!! ***!

        public DataTable GetProjectCategories()
        {
            sql = "select * from ProjectCategory"; // ALL TESTED
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetProjectDetails(string fypId)
        {
            sql = "select * from FYProject where fypId=" + fypId;
            return myOracleDBAccess.GetData(sql);
        }

        public DataTable GetProjectDigests()
        {
            sql = "select fypId, title, fypCategory, fypType, minStudents, maxStudents, isAvailable" +
                " from FYProject order by title";
            return myOracleDBAccess.GetData(sql);
        }

        public bool RemoveGroupFromProject(string groupId)
        {
            sql = "update ProjectGroup set fypAssigned=null where groupId=" + groupId;
            return SetData(sql);
        }

        public bool RemoveReader(string groupId)
        {
            sql = "update ProjectGroup set reader=null where groupId=" + groupId;
            return SetData(sql);
        }

        public bool SetData(string sql)
        {
            OracleTransaction trans = myOracleDBAccess.BeginTransaction();
            if (trans == null) { return false; }
            if (myOracleDBAccess.SetData(sql, trans))
            { myOracleDBAccess.CommitTransaction(trans); return true; } // The insert/update/delete succeeded.
            else
            { myOracleDBAccess.DisposeTransaction(trans); return false; } // The insert/update/delete failed.
        }
        #endregion
    }
}
