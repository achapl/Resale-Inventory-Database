-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: tool_database
-- ------------------------------------------------------
-- Server version	8.0.36

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
-- Table structure for table `item`
--

DROP TABLE IF EXISTS `item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `item` (
  `ITEM_ID` int unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `PurchaseID` int unsigned DEFAULT NULL,
  `ShippingID` int unsigned DEFAULT NULL,
  `InitialQuantity` int unsigned NOT NULL,
  `CurrentQuantity` int unsigned NOT NULL COMMENT 'Note: Keep signed in case of overflow error if an item of quantity 0 is subtracted one by some erroneous sale or otherwise. -1 is a good predictable consistant flag for an error',
  `Notes_item` mediumtext,
  `ThumbnailID` int unsigned DEFAULT NULL,
  PRIMARY KEY (`ITEM_ID`),
  UNIQUE KEY `ITEM_ID_UNIQUE` (`ITEM_ID`),
  KEY `PurchaseID_idx` (`Name`),
  KEY `PurchaseID_idx1` (`PurchaseID`),
  KEY `ShippingID_idx` (`ShippingID`),
  KEY `ThumbnailID_idx` (`ThumbnailID`),
  CONSTRAINT `PurchaseID` FOREIGN KEY (`PurchaseID`) REFERENCES `purchase` (`PURCHASE_ID`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `ShippingID` FOREIGN KEY (`ShippingID`) REFERENCES `shipping` (`SHIPPING_ID`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `ThumbnailID` FOREIGN KEY (`ThumbnailID`) REFERENCES `thumbnail` (`ThumbnailID`)
) ENGINE=InnoDB AUTO_INCREMENT=27208 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-02-16 22:35:29
