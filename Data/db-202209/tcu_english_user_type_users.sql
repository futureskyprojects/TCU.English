-- MySQL dump 10.13  Distrib 8.0.21, for Win64 (x86_64)
--
-- Host: 167.179.67.222    Database: tcu_english
-- ------------------------------------------------------
-- Server version	8.0.21

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `user_type_users`
--

DROP TABLE IF EXISTS `user_type_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_type_users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Active` tinyint(1) NOT NULL,
  `UpdatedTime` datetime(6) DEFAULT NULL,
  `CreatedTime` datetime(6) DEFAULT NULL,
  `UserId` int NOT NULL,
  `UserTypeId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_user_type_users_UserId` (`UserId`),
  KEY `IX_user_type_users_UserTypeId` (`UserTypeId`),
  CONSTRAINT `FK_user_type_users_user_types_UserTypeId` FOREIGN KEY (`UserTypeId`) REFERENCES `user_types` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_user_type_users_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_type_users`
--

LOCK TABLES `user_type_users` WRITE;
/*!40000 ALTER TABLE `user_type_users` DISABLE KEYS */;
INSERT INTO `user_type_users` VALUES (1,0,NULL,NULL,1,4),(3,1,'2020-09-10 04:13:35.221970','2020-09-10 04:13:35.221969',2,1),(4,1,'2020-09-17 14:54:23.528210','2020-09-17 14:54:23.528209',3,2),(5,1,'2020-09-19 07:44:44.974698','2020-09-19 07:44:44.974690',4,5);
/*!40000 ALTER TABLE `user_type_users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-22 20:55:17
