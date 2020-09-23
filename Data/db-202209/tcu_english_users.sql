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
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Active` tinyint(1) NOT NULL,
  `UpdatedTime` datetime(6) DEFAULT NULL,
  `CreatedTime` datetime(6) DEFAULT NULL,
  `Username` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(125) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `HashPassword` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FirstName` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Avatar` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Gender` int NOT NULL,
  `BirthDay` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,1,'2020-09-06 16:46:36.773560',NULL,'administration','administration@tcu.english.edu.vn','10000./1JCNQWG7b/ZLjZH9clGxg==.TgJjOXhmY/cb9xwJejh7N+yqX+JPzc7+WVjk8DHFwgY=','Hồ','Trương Phi','\\uploads\\administration\\images\\557A9E3BC1AC6985EA27C29D5FDF63995DB04AAC9957461ECD1976F2703EE73B_ad5e.jpg',1,'2020-08-23 00:00:00.000000'),(2,1,'2020-09-10 04:13:35.157794','2020-09-10 04:13:15.630589','hocvien1','phiho@gmail.com','10000.av04pKFYH1flO6tZZj/zlg==.GpJsXr9RgChgmVlJIgi2ox599M5Nj8NHgx3r7oqgqR4=','Học viên','01',NULL,1,'2020-09-10 00:00:00.000000'),(3,1,'2020-09-17 14:54:23.387842','2020-09-17 14:54:23.387730','quanlythuvien','quanlythuvien@gmail.com','10000.c1x5ak1vM3Ms0bI1Ynpzag==.YdEk140Mxst1Kk4AnlCJfywMJWyWB3ycjayMuHFyyGI=','Library','Manger','\\uploads\\quanlythuvien\\images\\672106528C26D992EFFD7146030386D2D52F1F9E6FEAD53203D37844C0F008E5_0d8f.png',1,'2020-09-17 00:00:00.000000'),(4,1,'2020-09-19 07:44:44.072894','2020-09-19 07:44:44.072661','instructor','instructor@vistark.vn','10000.tDDTpIF258yjulZcScQH9g==.lmR+TBTof7BSzhCMIIbKH3qSBH9VjlmHytodYBSwN2g=','Phi Hồ','Instructor',NULL,1,'2020-09-19 00:00:00.000000');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-22 20:54:44
