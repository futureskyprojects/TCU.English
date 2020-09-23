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
-- Table structure for table `writing_part_1`
--

DROP TABLE IF EXISTS `writing_part_1`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `writing_part_1` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Active` tinyint(1) NOT NULL,
  `UpdatedTime` datetime(6) DEFAULT NULL,
  `CreatedTime` datetime(6) DEFAULT NULL,
  `DefaultSentence` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SecondSentence` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Hint` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Answers` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ExplainLink` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatorId` int NOT NULL,
  `TestCategoryId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_writing_part_1_CreatorId` (`CreatorId`),
  KEY `IX_writing_part_1_TestCategoryId` (`TestCategoryId`),
  CONSTRAINT `FK_writing_part_1_test_categories_TestCategoryId` FOREIGN KEY (`TestCategoryId`) REFERENCES `test_categories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_writing_part_1_users_CreatorId` FOREIGN KEY (`CreatorId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `writing_part_1`
--

LOCK TABLES `writing_part_1` WRITE;
/*!40000 ALTER TABLE `writing_part_1` DISABLE KEYS */;
INSERT INTO `writing_part_1` VALUES (5,1,'2020-09-13 15:25:34.974479','2020-09-13 15:12:27.000000','Nick was given a ticket to a baseball game by his friend Akio.','Nick\'s friend Akio …………….. him a ticket to a baseball game.','passive-active: be + V_3=V; ','[{\"AnswerContent\":\"gave\",\"IsCorrect\":false},{\"AnswerContent\":\" gave \",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]','was given = gave',1,34),(6,1,'2020-09-13 15:22:52.803980','2020-09-13 15:22:52.803953','Nick had never been to a baseball game before.','It was the  ……………… Nick had been to a baseball game.','had never done before= It was the first time had done','[{\"AnswerContent\":\"first time\",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]',NULL,1,34),(7,1,'2020-09-13 15:23:53.936983','2020-09-13 15:23:53.936976','Nick wasn\'t sure when the match would finish.','Nick wasn\'t sure ………….. long the match would go on for.','when= how long','[{\"AnswerContent\":\"how\",\"IsCorrect\":false},{\"AnswerContent\":\"how \",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]',NULL,1,34),(8,1,'2020-09-13 15:27:37.770581','2020-09-13 15:27:37.770565','In the game the Dolphins team played better than the Giants.','In the game the Giants team didn\'t play as ……………... the Dolphins.','better than= not as well as','[{\"AnswerContent\":\"well as\",\"IsCorrect\":false},{\"AnswerContent\":\" well as \",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]',NULL,1,34),(9,1,'2020-09-13 15:29:03.252619','2020-09-13 15:29:03.252594','Now Nick can\'t wait for the next game.','Now Nick is really looking .………………. to the next game.','wait for= be looking forward to + V-ing','[{\"AnswerContent\":\"forward\",\"IsCorrect\":false},{\"AnswerContent\":\" forward \",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]',NULL,1,34),(10,1,'2020-09-13 15:38:56.541692','2020-09-13 15:36:38.000000','I often go cycling with my friend Dan at the weekends.','My friend Dan usually ……….. cycling with me at the weekends.','A with B= B with A','[{\"AnswerContent\":\"goes\",\"IsCorrect\":false},{\"AnswerContent\":\"comes\",\"IsCorrect\":false},{\"AnswerContent\":\" goes \",\"IsCorrect\":false},{\"AnswerContent\":\" comes \",\"IsCorrect\":false}]',NULL,1,34),(11,1,'2020-09-13 15:40:09.860946','2020-09-13 15:40:09.860933','Our favourite place to visit is the lake near our town.','We like visiting the lake near our town ………… than anywhere else.','favourite= like more/ better than','[{\"AnswerContent\":\"better\",\"IsCorrect\":false},{\"AnswerContent\":\" better \",\"IsCorrect\":false},{\"AnswerContent\":\"more\",\"IsCorrect\":false},{\"AnswerContent\":\" more \",\"IsCorrect\":false}]',NULL,1,36),(12,1,'2020-09-13 15:41:10.565008','2020-09-13 15:41:10.564995','We hadn\'t cycled into the countryside for several months.','It was several months …………… we had cycled into the countryside.','for a period of time= since a starting point','[{\"AnswerContent\":\"since\",\"IsCorrect\":false},{\"AnswerContent\":\" since \",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]',NULL,1,36),(13,1,'2020-09-13 15:42:04.764576','2020-09-13 15:42:04.764575','We only had a break when we got hungry at about 2.00.','We didn\'t have a break …………… we got hungry at about 2.00.','only do s.t when = do not do s.t until …','[{\"AnswerContent\":\"until\",\"IsCorrect\":false},{\"AnswerContent\":\" until \",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]',NULL,1,36),(14,1,'2020-09-13 15:42:46.663453','2020-09-13 15:42:46.663453','It was the best ride we\'d had for a long time.','We hadn\'t had ……………………… a good ride for a long time.','It’s the best= such a NP: Adj+ Noun','[{\"AnswerContent\":\"such\",\"IsCorrect\":false},{\"AnswerContent\":\" such \",\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false},{\"AnswerContent\":null,\"IsCorrect\":false}]',NULL,1,36);
/*!40000 ALTER TABLE `writing_part_1` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-22 20:56:29
