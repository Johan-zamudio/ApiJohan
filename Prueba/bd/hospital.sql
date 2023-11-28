-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1:3306
-- Tiempo de generación: 28-11-2023 a las 23:54:39
-- Versión del servidor: 8.0.31
-- Versión de PHP: 8.0.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `hospital`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `medico`
--

DROP TABLE IF EXISTS `medico`;
CREATE TABLE IF NOT EXISTS `medico` (
  `idMedico` int NOT NULL AUTO_INCREMENT,
  `NombreMed` varchar(50) NOT NULL,
  `ApellidoMed` varchar(50) NOT NULL,
  `RunMed` varchar(50) NOT NULL,
  `Eunacom` char(5) NOT NULL,
  `NacionalidadMed` varchar(45) NOT NULL,
  `Especialidad` varchar(45) NOT NULL,
  `horarios` time NOT NULL,
  `TarifaHr` int NOT NULL,
  PRIMARY KEY (`idMedico`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;

--
-- Volcado de datos para la tabla `medico`
--

INSERT INTO `medico` (`idMedico`, `NombreMed`, `ApellidoMed`, `RunMed`, `Eunacom`, `NacionalidadMed`, `Especialidad`, `horarios`, `TarifaHr`) VALUES
(1, 'Johan Andres', 'Zamudio', '23160334-4', 'si', 'Colombiana', 'Pediatria', '23:24:00', 2300);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `paciente`
--

DROP TABLE IF EXISTS `paciente`;
CREATE TABLE IF NOT EXISTS `paciente` (
  `idPaciente` int NOT NULL AUTO_INCREMENT,
  `NombrePac` varchar(50) NOT NULL,
  `ApellidoPac` varchar(50) NOT NULL,
  `RunPac` int NOT NULL,
  `Nacionalidad` varchar(50) NOT NULL,
  `Visa` varchar(5) NOT NULL,
  `Genero` varchar(10) NOT NULL,
  `SistomasPac` varchar(100) NOT NULL,
  `Medico_idMedico` int NOT NULL,
  PRIMARY KEY (`idPaciente`),
  KEY `Medico_idMedico` (`Medico_idMedico`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;

--
-- Volcado de datos para la tabla `paciente`
--

INSERT INTO `paciente` (`idPaciente`, `NombrePac`, `ApellidoPac`, `RunPac`, `Nacionalidad`, `Visa`, `Genero`, `SistomasPac`, `Medico_idMedico`) VALUES
(1, 'Juanito', 'Perez', 23160334, 'Chileno', 'Si', 'Masculino', 'Resfrio y fiebre', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reserva`
--

DROP TABLE IF EXISTS `reserva`;
CREATE TABLE IF NOT EXISTS `reserva` (
  `idReserva` int NOT NULL AUTO_INCREMENT,
  `Especialidad` varchar(45) NOT NULL,
  `DiaReserva` varchar(10) NOT NULL,
  `Paciente_idPaciente` int NOT NULL,
  PRIMARY KEY (`idReserva`),
  KEY `Paciente_idPaciente` (`Paciente_idPaciente`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;

--
-- Volcado de datos para la tabla `reserva`
--

INSERT INTO `reserva` (`idReserva`, `Especialidad`, `DiaReserva`, `Paciente_idPaciente`) VALUES
(1, 'Pediatria', '0000-00-00', 1);

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `paciente`
--
ALTER TABLE `paciente`
  ADD CONSTRAINT `paciente_ibfk_1` FOREIGN KEY (`Medico_idMedico`) REFERENCES `medico` (`idMedico`);

--
-- Filtros para la tabla `reserva`
--
ALTER TABLE `reserva`
  ADD CONSTRAINT `reserva_ibfk_1` FOREIGN KEY (`Paciente_idPaciente`) REFERENCES `paciente` (`idPaciente`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
