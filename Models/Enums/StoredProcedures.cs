﻿
namespace Models.Enums
{
    public enum StoredProcedures
    {
        //---Activity Logs---//
        tblActivityLogs_GetAll,
        tblActivityLogs_GetById,
        tblActivityLogs_Insert,
        tblActivityLogs_Update,
        tblActivityLogs_Delete,

        //---Backup Records---//
        tblBackupRecords_GetAll,
        tblBackupRecords_GetById,
        tblBackupRecords_Insert,
        tblBackupRecords_Update,
        tblBackupRecords_Delete,

        //---Contracts---//
        tblContracts_GetAll,
        tblContracts_GetById,
        tblContracts_Insert,
        tblContracts_Update,
        tblContracts_Delete,

        //---Department---//
        tblDepartment_GetAll,
        tblDepartment_GetById,
        tblDepartment_Insert,
        tblDepartment_Update,
        tblDepartment_Delete,

        //---Employee---//
        tblEmployees_GetAll,
        tblEmployees_GetById,
        tblEmployees_Insert,
        tblEmployees_Update,
        tblEmployees_Delete,

        //---Notifications---//
        tblNotifications_GetAll,
        tblNotifications_GetById,
        tblNotifications_Insert,
        tblNotifications_Update,
        tblNotifications_Delete,

        //---Positions---//
        tblPositions_GetAll,
        tblPositions_GetById,
        tblPositions_Insert,
        tblPositions_Update,
        tblPositions_Delete,

        //--tblUser--//
        tblUsers_GetAll,
        tblUsers_GetById,
        tblUsers_Insert,
        tblUsers_GetByUsername,
        tblUsers_Update,
        tblUsers_Delete,

        //--tblRole--//
        tblRoles_GetByUserId,
    }
}
