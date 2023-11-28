using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// Definición del enrutamiento base para las acciones del controlador
[Route("Reserva")]
[ApiController]
public class ReservaController : ControllerBase
{
    // Variable para almacenar la cadena de conexión a la base de datos
    private readonly string _connectionString;

    // Constructor del controlador que recibe la configuración (appsettings.json)
    public ReservaController(IConfiguration config)
    {
        // Obtener la cadena de conexión desde la configuración
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // Acción para obtener la lista de reservas
    [HttpGet]
    public async Task<IActionResult> ListarReservas()
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para obtener todos los registros de la tabla Reserva
                string query = "SELECT * FROM Reserva";

                // Lista para almacenar objetos de tipo Reserva
                List<Reserva> reservas = new List<Reserva>();

                // Ejecutar la consulta y leer los resultados de manera asíncrona
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Recorrer cada fila de resultados
                    while (await reader.ReadAsync())
                    {
                        // Crear un objeto Reserva y agregarlo a la lista
                        reservas.Add(new Reserva
                        {
                            idReserva = reader.GetInt32(0),
                            Especialidad = reader.GetString(1),
                            DiaReserva = reader.GetString(2),
                            Paciente_idPaciente = reader.GetInt32(3)
                        });
                    }
                }

                // Devolver una respuesta HTTP 200 con la lista de reservas
                return StatusCode(200, reservas);
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se pudo listar los registros por: " + ex);
        }
    }

    // Acción para obtener una reserva específica por su ID
    [HttpGet("{idReserva}")]
    public async Task<IActionResult> ObtenerReserva(int id)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para obtener una reserva por su ID
                string query = "SELECT * FROM Reserva WHERE idReserva = @id";

                // Objeto para almacenar una reserva específica
                Reserva reserva = new Reserva();

                // Ejecutar la consulta con un parámetro de ID y leer los resultados de manera asíncrona
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignar el valor del parámetro ID
                    command.Parameters.AddWithValue("@id", id);

                    // Leer los resultados
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Verificar si se encontró un registro
                        if (await reader.ReadAsync())
                        {
                            // Asignar los valores de la reserva encontrada al objeto Reserva
                            reserva.idReserva = reader.GetInt32(0);
                            reserva.Especialidad = reader.GetString(1);
                            reserva.DiaReserva = reader.GetString(2);
                            reserva.Paciente_idPaciente = reader.GetInt32(3);

                            // Devolver una respuesta HTTP 200 con la reserva encontrada
                            return StatusCode(200, reserva);
                        }
                        else
                        {
                            // Devolver una respuesta HTTP 404 si no se encuentra el registro
                            return StatusCode(404, "No se encuentra el registro");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se puede realizar la petición por: " + ex);
        }
    }

    // Acción para crear una nueva reserva
    [HttpPost]
    public async Task<IActionResult> CrearReserva([FromBody] Reserva reserva)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para insertar un nuevo registro de reserva
                string query = "INSERT INTO Reserva (Especialidad, DiaReserva, Paciente_idPaciente) VALUES (@Especialidad, @DiaReserva, @Paciente_idPaciente)";

                // Ejecutar la consulta con parámetros y valores de la nueva reserva
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignar valores de los parámetros
                    command.Parameters.AddWithValue("@Especialidad", reserva.Especialidad);
                    command.Parameters.AddWithValue("@DiaReserva", reserva.DiaReserva);
                    command.Parameters.AddWithValue("@Paciente_idPaciente", reserva.Paciente_idPaciente);

                    // Ejecutar la consulta de inserción de manera asíncrona
                    await command.ExecuteNonQueryAsync();

                    // Devolver una respuesta HTTP 201 indicando que la reserva fue creada correctamente
                    return StatusCode(201, $"Reserva creada correctamente: {reserva}");
                }
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se pudo guardar el registro por: " + ex);
        }
    }

    // Acción para editar la información de una reserva existente
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarReserva(int id, [FromBody] Reserva reserva)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para actualizar la información de una reserva por su ID
                string query = "UPDATE Reserva SET Especialidad = @Especialidad, DiaReserva = @DiaReserva, Paciente_idPaciente = @Paciente_idPaciente WHERE idReserva = @id";

                // Ejecutar la consulta con parámetros y valores actualizados de la reserva
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignar valores de los parámetros
                    command.Parameters.AddWithValue("@Especialidad", reserva.Especialidad);
                    command.Parameters.AddWithValue("@DiaReserva", reserva.DiaReserva);
                    command.Parameters.AddWithValue("@Paciente_idPaciente", reserva.Paciente_idPaciente);
                    command.Parameters.AddWithValue("@id", id);

                    // Ejecutar la consulta de actualización de manera asíncrona
                    await command.ExecuteNonQueryAsync();

                    // Devolver una respuesta HTTP 200 indicando que la reserva fue editada correctamente
                    return StatusCode(200, "Registro editado correctamente");
                }
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se pudo editar la reserva por: " + ex);
        }
    }

    // Acción para eliminar una reserva por su ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarReserva(int id)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para eliminar una reserva por su ID
                string query = "DELETE FROM Reserva WHERE idReserva = @id";

                // Ejecutar la consulta con el parámetro de ID a eliminar
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignar el valor del parámetro ID
                    command.Parameters.AddWithValue("@id", id);

                    // Ejecutar la consulta de eliminación de manera asíncrona y obtener el resultado
                    var borrado = await command.ExecuteNonQueryAsync();

                    // Verificar si se encontró y eliminó el registro
                    if (borrado == 0)
                    {
                        // Devolver una respuesta HTTP 404 si no se encuentra el registro
                        return StatusCode(404, "Registro no encontrado!!!");
                    }
                    else
                    {
                        // Devolver una respuesta HTTP 200 indicando que la reserva fue eliminada correctamente
                        return StatusCode(200, $"Reserva con el ID {id} eliminada correctamente");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se pudo eliminar el registro por: " + ex);
        }
    }
}
