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
-- Table structure for table `speaking_embed`
--

DROP TABLE IF EXISTS `speaking_embed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `speaking_embed` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Active` tinyint(1) NOT NULL,
  `UpdatedTime` datetime(6) DEFAULT NULL,
  `CreatedTime` datetime(6) DEFAULT NULL,
  `YoutubeVideo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatorId` int NOT NULL,
  `TestCategoryId` int NOT NULL,
  `Hint` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_speaking_embed_CreatorId` (`CreatorId`),
  KEY `IX_speaking_embed_TestCategoryId` (`TestCategoryId`),
  CONSTRAINT `FK_speaking_embed_test_categories_TestCategoryId` FOREIGN KEY (`TestCategoryId`) REFERENCES `test_categories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_speaking_embed_users_CreatorId` FOREIGN KEY (`CreatorId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `speaking_embed`
--

LOCK TABLES `speaking_embed` WRITE;
/*!40000 ALTER TABLE `speaking_embed` DISABLE KEYS */;
INSERT INTO `speaking_embed` VALUES (1,1,'2020-09-19 07:31:51.502522','2020-09-19 06:38:06.000000','https://www.youtube.com/watch?v=PR_yVho1Txc',1,102,NULL);
/*!40000 ALTER TABLE `speaking_embed` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-22 20:54:51
