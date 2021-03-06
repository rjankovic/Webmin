-- phpMyAdmin SQL Dump
-- version 3.3.2deb1ubuntu1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jul 07, 2013 at 05:11 PM
-- Server version: 5.1.69
-- PHP Version: 5.3.2-1ubuntu4.19

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT=0;
START TRANSACTION;


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `webmin`
--

-- --------------------------------------------------------

--
-- Table structure for table `access_rights`
--

CREATE TABLE IF NOT EXISTS `access_rights` (
  `id_access_rights` int(11) NOT NULL AUTO_INCREMENT,
  `id_user` int(11) NOT NULL,
  `id_project` int(11) DEFAULT NULL,
  `access` int(11) NOT NULL,
  PRIMARY KEY (`id_access_rights`),
  UNIQUE KEY `id_user_2` (`id_user`,`id_project`),
  KEY `id_user` (`id_user`),
  KEY `id_project` (`id_project`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=41 ;

--
-- Dumping data for table `access_rights`
--


-- --------------------------------------------------------

--
-- Table structure for table `controls`
--

CREATE TABLE IF NOT EXISTS `controls` (
  `id_control` int(11) NOT NULL AUTO_INCREMENT,
  `id_panel` int(11) NOT NULL,
  `content` longtext NOT NULL,
  PRIMARY KEY (`id_control`),
  KEY `id_panel` (`id_panel`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=10958 ;

--
-- Dumping data for table `controls`
--


-- --------------------------------------------------------

--
-- Table structure for table `fields`
--

CREATE TABLE IF NOT EXISTS `fields` (
  `id_field` int(11) NOT NULL AUTO_INCREMENT,
  `id_panel` int(11) NOT NULL,
  `content` longtext NOT NULL,
  PRIMARY KEY (`id_field`),
  KEY `id_panel` (`id_panel`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=17972 ;

--
-- Dumping data for table `fields`
--


-- --------------------------------------------------------

--
-- Table structure for table `hierarchy_nav_tables`
--

CREATE TABLE IF NOT EXISTS `hierarchy_nav_tables` (
  `id_item` int(11) NOT NULL,
  `id_control` int(11) NOT NULL,
  `id_parent` int(11) DEFAULT NULL,
  `caption` varchar(255) NOT NULL,
  `id_nav` int(11) DEFAULT NULL,
  PRIMARY KEY (`id_item`,`id_control`),
  KEY `id_control` (`id_control`),
  KEY `id_parent` (`id_parent`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `hierarchy_nav_tables`
--


-- --------------------------------------------------------

--
-- Table structure for table `locks`
--

CREATE TABLE IF NOT EXISTS `locks` (
  `id_lock` int(11) NOT NULL AUTO_INCREMENT,
  `id_owner` int(11) NOT NULL,
  `id_project` int(11) NOT NULL,
  `lock_type` int(11) NOT NULL,
  PRIMARY KEY (`id_lock`),
  UNIQUE KEY `id_project_2` (`id_project`,`lock_type`),
  KEY `id_project` (`id_project`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=153 ;

--
-- Dumping data for table `locks`
--


-- --------------------------------------------------------

--
-- Table structure for table `mysql_Membership`
--

CREATE TABLE IF NOT EXISTS `mysql_Membership` (
  `PKID` varchar(36) NOT NULL,
  `Username` varchar(255) NOT NULL,
  `ApplicationName` varchar(255) NOT NULL,
  `Email` varchar(128) DEFAULT NULL,
  `Comment` varchar(255) DEFAULT NULL,
  `Password` varchar(128) NOT NULL,
  `PasswordKey` char(32) DEFAULT NULL,
  `PasswordFormat` tinyint(4) DEFAULT NULL,
  `PasswordQuestion` varchar(255) DEFAULT NULL,
  `PasswordAnswer` varchar(255) DEFAULT NULL,
  `IsApproved` tinyint(1) DEFAULT NULL,
  `LastActivityDate` datetime DEFAULT NULL,
  `LastLoginDate` datetime DEFAULT NULL,
  `LastPasswordChangedDate` datetime DEFAULT NULL,
  `CreationDate` datetime DEFAULT NULL,
  `IsOnline` tinyint(1) DEFAULT NULL,
  `IsLockedOut` tinyint(1) DEFAULT NULL,
  `LastLockedOutDate` datetime DEFAULT NULL,
  `FailedPasswordAttemptCount` int(10) unsigned DEFAULT NULL,
  `FailedPasswordAttemptWindowStart` datetime DEFAULT NULL,
  `FailedPasswordAnswerAttemptCount` int(10) unsigned DEFAULT NULL,
  `FailedPasswordAnswerAttemptWindowStart` datetime DEFAULT NULL,
  PRIMARY KEY (`PKID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COMMENT='2';

--
-- Dumping data for table `mysql_Membership`
--


-- --------------------------------------------------------

--
-- Table structure for table `my_aspnet_applications`
--

CREATE TABLE IF NOT EXISTS `my_aspnet_applications` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(256) DEFAULT NULL,
  `description` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `my_aspnet_applications`
--

INSERT INTO `my_aspnet_applications` (`id`, `name`, `description`) VALUES
(2, '/', 'MySQL default application');

-- --------------------------------------------------------

--
-- Table structure for table `my_aspnet_membership`
--

CREATE TABLE IF NOT EXISTS `my_aspnet_membership` (
  `userId` int(11) NOT NULL DEFAULT '0',
  `Email` varchar(128) DEFAULT NULL,
  `Comment` varchar(255) DEFAULT NULL,
  `Password` varchar(128) NOT NULL,
  `PasswordKey` char(32) DEFAULT NULL,
  `PasswordFormat` tinyint(4) DEFAULT NULL,
  `PasswordQuestion` varchar(255) DEFAULT NULL,
  `PasswordAnswer` varchar(255) DEFAULT NULL,
  `IsApproved` tinyint(1) DEFAULT NULL,
  `LastActivityDate` datetime DEFAULT NULL,
  `LastLoginDate` datetime DEFAULT NULL,
  `LastPasswordChangedDate` datetime DEFAULT NULL,
  `CreationDate` datetime DEFAULT NULL,
  `IsLockedOut` tinyint(1) DEFAULT NULL,
  `LastLockedOutDate` datetime DEFAULT NULL,
  `FailedPasswordAttemptCount` int(10) unsigned DEFAULT NULL,
  `FailedPasswordAttemptWindowStart` datetime DEFAULT NULL,
  `FailedPasswordAnswerAttemptCount` int(10) unsigned DEFAULT NULL,
  `FailedPasswordAnswerAttemptWindowStart` datetime DEFAULT NULL,
  PRIMARY KEY (`userId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='2';

--
-- Dumping data for table `my_aspnet_membership`
--


-- --------------------------------------------------------

--
-- Table structure for table `my_aspnet_schemaversion`
--

CREATE TABLE IF NOT EXISTS `my_aspnet_schemaversion` (
  `version` int(11) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `my_aspnet_schemaversion`
--

INSERT INTO `my_aspnet_schemaversion` (`version`) VALUES
(7);

-- --------------------------------------------------------

--
-- Table structure for table `my_aspnet_sessioncleanup`
--

CREATE TABLE IF NOT EXISTS `my_aspnet_sessioncleanup` (
  `LastRun` datetime NOT NULL,
  `IntervalMinutes` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `my_aspnet_sessioncleanup`
--


-- --------------------------------------------------------

--
-- Table structure for table `my_aspnet_sessions`
--

CREATE TABLE IF NOT EXISTS `my_aspnet_sessions` (
  `SessionId` varchar(255) NOT NULL,
  `ApplicationId` int(11) NOT NULL,
  `Created` datetime NOT NULL,
  `Expires` datetime NOT NULL,
  `LockDate` datetime NOT NULL,
  `LockId` int(11) NOT NULL,
  `Timeout` int(11) NOT NULL,
  `Locked` tinyint(1) NOT NULL,
  `SessionItems` longblob,
  `Flags` int(11) NOT NULL,
  PRIMARY KEY (`SessionId`,`ApplicationId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `my_aspnet_sessions`
--


-- --------------------------------------------------------

--
-- Table structure for table `my_aspnet_users`
--

CREATE TABLE IF NOT EXISTS `my_aspnet_users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `applicationId` int(11) NOT NULL,
  `name` varchar(256) NOT NULL,
  `isAnonymous` tinyint(1) NOT NULL DEFAULT '1',
  `lastActivityDate` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

--
-- Dumping data for table `my_aspnet_users`
--


-- --------------------------------------------------------

--
-- Table structure for table `my_aspnet_usersinroles`
--

CREATE TABLE IF NOT EXISTS `my_aspnet_usersinroles` (
  `userId` int(11) NOT NULL DEFAULT '0',
  `roleId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`userId`,`roleId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

--
-- Dumping data for table `my_aspnet_usersinroles`
--


-- --------------------------------------------------------

--
-- Table structure for table `panels`
--

CREATE TABLE IF NOT EXISTS `panels` (
  `id_panel` int(11) NOT NULL AUTO_INCREMENT,
  `id_project` int(11) NOT NULL,
  `id_parent` int(11) DEFAULT NULL,
  `id_holder` int(11) DEFAULT NULL,
  `content` longtext NOT NULL,
  PRIMARY KEY (`id_panel`),
  KEY `id_project` (`id_project`),
  KEY `id_holder` (`id_holder`),
  KEY `id_parent` (`id_parent`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=5082 ;

--
-- Dumping data for table `panels`
--


--
-- Constraints for dumped tables
--

--
-- Constraints for table `controls`
--
ALTER TABLE `controls`
  ADD CONSTRAINT `controls_ibfk_1` FOREIGN KEY (`id_panel`) REFERENCES `panels` (`id_panel`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `fields`
--
ALTER TABLE `fields`
  ADD CONSTRAINT `fields_ibfk_1` FOREIGN KEY (`id_panel`) REFERENCES `panels` (`id_panel`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `hierarchy_nav_tables`
--
ALTER TABLE `hierarchy_nav_tables`
  ADD CONSTRAINT `hierarchy_nav_tables_ibfk_1` FOREIGN KEY (`id_control`) REFERENCES `controls` (`id_control`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `panels`
--
ALTER TABLE `panels`
  ADD CONSTRAINT `panels_ibfk_3` FOREIGN KEY (`id_parent`) REFERENCES `panels` (`id_panel`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `panels_ibfk_4` FOREIGN KEY (`id_holder`) REFERENCES `fields` (`id_field`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;
