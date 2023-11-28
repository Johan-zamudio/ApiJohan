using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// Definición del enrutamiento base para las acciones del controlador
[Route("Paciente")]
[ApiController]
public class PacienteController : ControllerBase
{
    // Variable para almacenar la cadena de conexión a la base de datos
    private readonly string _connectionString;

    // Constructor del controlador que recibe la configuración (appsettings.json)
    public PacienteController(IConfiguration config)
    {
        // Obtener la cadena de conexión desde la configuración
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // Acción para obtener la lista de pacientes
    [HttpGet]
    public async Task<IActionResult> ListarPacientes()
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para obtener todos los registros de la tabla Paciente
                string query = "SELECT * FROM Paciente";

                // Lista para almacenar objetos de tipo Paciente
                List<Paciente> pacientes = new List<Paciente>();

                // Ejecutar la consulta y leer los resultados de manera asíncrona
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Recorrer cada fila de resultados
                    while (await reader.ReadAsync())
                    {
                        // Crear un objeto Paciente y agregarlo a la lista
                        pacientes.Add(new Paciente
                        {
                            idPaciente = reader.GetInt32(0),
                            NombrePac = reader.GetString(1),
                            ApellidoPac = reader.GetString(2),
                            RunPac = reader.GetString(3),
                            Nacionalidad = reader.GetString(4),
                            Visa = reader.GetString(5),
                            Genero = reader.GetString(6),
                            SistomasPac = reader.GetString(7),
                            Medico_idMedico = reader.GetInt32(8)
                        });
                    }
                }

                // Devolver una respuesta HTTP 200 con la lista de pacientes
                return StatusCode(200, pacientes);
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se pudo listar los registros por: " + ex);
        }
    }

    // Acción para obtener un paciente específico por su ID
    [HttpGet("{idPaciente}")]
    public async Task<IActionResult> ObtenerPaciente(int id)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para obtener un paciente por su ID
                string query = "SELECT * FROM Paciente WHERE idPaciente = @id";

                // Objeto para almacenar un paciente específico
                Paciente paciente = new Paciente();

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
                            // Asignar los valores del paciente encontrado al objeto Paciente
                            paciente.idPaciente = reader.GetInt32(0);
                            paciente.NombrePac = reader.GetString(1);
                            paciente.ApellidoPac = reader.GetString(2);
                            paciente.RunPac = reader.GetString(3);
                            paciente.Nacionalidad = reader.GetString(4);
                            paciente.Visa = reader.GetString(5);
                            paciente.Genero = reader.GetString(6);
                            paciente.SistomasPac = reader.GetString(7);
                            paciente.Medico_idMedico = reader.GetInt32(8);

                            // Devolver una respuesta HTTP 200 con el paciente encontrado
                            return StatusCode(200, paciente);
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

    // Acción para crear un nuevo paciente
    [HttpPost]
    public async Task<IActionResult> CrearPaciente([FromBody] Paciente paciente)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para insertar un nuevo registro de paciente
                string query = "INSERT INTO Paciente (NombrePac, ApellidoPac, RunPac, Nacionalidad, Visa, Genero, SistomasPac, Medico_idMedico) VALUES (@NombrePac, @ApellidoPac, @RunPac, @Nacionalidad, @Visa, @Genero, @SistomasPac, @Medico_idMedico)";

                // Ejecutar la consulta con parámetros y valores del nuevo paciente
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignar valores de los parámetros
                    command.Parameters.AddWithValue("@NombrePac", paciente.NombrePac);
                    command.Parameters.AddWithValue("@ApellidoPac", paciente.ApellidoPac);
                    command.Parameters.AddWithValue("@RunPac", paciente.RunPac);
                    command.Parameters.AddWithValue("@Nacionalidad", paciente.Nacionalidad);
                    command.Parameters.AddWithValue("@Visa", paciente.Visa);
                    command.Parameters.AddWithValue("@Genero", paciente.Genero);
                    command.Parameters.AddWithValue("@SistomasPac", paciente.SistomasPac);
                    command.Parameters.AddWithValue("@Medico_idMedico", paciente.Medico_idMedico);

                    // Ejecutar la consulta de inserción de manera asíncrona
                    await command.ExecuteNonQueryAsync();

                    // Devolver una respuesta HTTP 201 indicando que el paciente fue creado correctamente
                    return StatusCode(201, $"Paciente creado correctamente: {paciente}");
                }
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se pudo guardar el registro por: " + ex);
        }
    }

    // Acción para editar la información de un paciente existente
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarPaciente(int id, [FromBody] Paciente paciente)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para actualizar la información de un paciente por su ID
                string query = "UPDATE Paciente SET NombrePac = @NombrePac, ApellidoPac = @ApellidoPac, RunPac = @RunPac, Nacionalidad = @Nacionalidad, Visa = @Visa, Genero = @Genero, SistomasPac = @SistomasPac, Medico_idMedico = @Medico_idMedico WHERE idPaciente = @id";

                // Ejecutar la consulta con parámetros y valores actualizados del paciente
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignar valores de los parámetros
                    command.Parameters.AddWithValue("@NombrePac", paciente.NombrePac);
                    command.Parameters.AddWithValue("@ApellidoPac", paciente.ApellidoPac);
                    command.Parameters.AddWithValue("@RunPac", paciente.RunPac);
                    command.Parameters.AddWithValue("@Nacionalidad", paciente.Nacionalidad);
                    command.Parameters.AddWithValue("@Visa", paciente.Visa);
                    command.Parameters.AddWithValue("@Genero", paciente.Genero);
                    command.Parameters.AddWithValue("@SistomasPac", paciente.SistomasPac);
                    command.Parameters.AddWithValue("@Medico_idMedico", paciente.Medico_idMedico);
                    command.Parameters.AddWithValue("@id", id);

                    // Ejecutar la consulta de actualización de manera asíncrona
                    await command.ExecuteNonQueryAsync();

                    // Devolver una respuesta HTTP 200 indicando que el paciente fue editado correctamente
                    return StatusCode(200, "Registro editado correctamente");
                }
            }
        }
        catch (Exception ex)
        {
            // Devolver una respuesta HTTP 500 en caso de error
            return StatusCode(500, "No se pudo editar el paciente por: " + ex);
        }
    }

    // Acción para eliminar un paciente por su ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarPaciente(int id)
    {
        try
        {
            // Establecer la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                // Abrir la conexión de manera asíncrona
                await connection.OpenAsync();

                // Consulta SQL para eliminar un paciente por su ID
                string query = "DELETE FROM Paciente WHERE idPaciente = @id";

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
                        // Devolver una respuesta HTTP 200 indicando que el paciente fue eliminado correctamente
                        return StatusCode(200, $"Paciente con el ID {id} eliminado correctamente");
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
