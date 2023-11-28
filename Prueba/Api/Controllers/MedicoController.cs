// Importación de namespaces necesarios
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// Definición del controlador para la entidad "Medico"
[Route("Medico")]
[ApiController]
public class MedicoController : ControllerBase
{
    // Cadena de conexión a la base de datos
    private readonly string _connectionString;

    // Constructor del controlador que recibe la configuración (por inyección de dependencias)
    public MedicoController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // Método para manejar solicitudes GET para listar todos los médicos
    [HttpGet]
    public async Task<IActionResult> ListarMedicos()
    {
        try
        {
            // Uso de "using" para garantizar la liberación de recursos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta SQL para obtener todos los registros de la tabla "Medico"
                string query = "SELECT * FROM Medico";

                // Lista para almacenar objetos de tipo "Medico"
                List<Medico> medicos = new List<Medico>();

                // Uso de "using" para garantizar la liberación de recursos
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Iteración a través de los registros devueltos por la consulta
                    while (await reader.ReadAsync())
                    {
                        // Creación de un objeto "Medico" y asignación de sus propiedades desde el lector
                        medicos.Add(new Medico
                        {
                            idMedico = reader.GetInt32(0),
                            NombreMed = reader.GetString(1),
                            ApellidoMed = reader.GetString(2),
                            RunMed = reader.GetString(3),
                            Eunacom = reader.GetString(4),
                            NacionalidadMed = reader.GetString(5),
                            Especialidad = reader.GetString(6),
                            horarios = reader.GetString(7),
                            TarifaHr = reader.GetInt32(8)
                        });
                    }
                }

                // Devolución de una respuesta HTTP 200 con la lista de médicos en formato JSON
                return StatusCode(200, medicos);
            }
        }
        catch (Exception ex)
        {
            // Devolución de una respuesta HTTP 500 en caso de error, con detalles del error
            return StatusCode(500, "No se pudo listar los registros por: " + ex);
        }
    }

    // Método para manejar solicitudes GET para obtener un médico por su ID
    [HttpGet("{idMedico}")]
    public async Task<IActionResult> ObtenerMedico(int id)
    {
        try
        {
            // Uso de "using" para garantizar la liberación de recursos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta SQL para obtener un médico por su ID
                string query = "SELECT * FROM Medico WHERE idMedico = @id";

                // Objeto "Medico" para almacenar el resultado
                Medico medico = new Medico();

                // Uso de "using" para garantizar la liberación de recursos
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    // Uso de "using" para garantizar la liberación de recursos
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Verificación de si hay un registro disponible en el lector
                        if (await reader.ReadAsync())
                        {
                            // Asignación de propiedades del objeto "Medico" desde el lector
                            medico.idMedico = reader.GetInt32(0);
                            medico.NombreMed = reader.GetString(1);
                            medico.ApellidoMed = reader.GetString(2);
                            medico.RunMed = reader.GetString(3);
                            medico.Eunacom = reader.GetString(4);
                            medico.NacionalidadMed = reader.GetString(5);
                            medico.Especialidad = reader.GetString(6);
                            medico.horarios = reader.GetString(7);
                            medico.TarifaHr = reader.GetInt32(8);

                            // Devolución de una respuesta HTTP 200 con el objeto "Medico" en formato JSON
                            return StatusCode(200, medico);
                        }
                        else
                        {
                            // Devolución de una respuesta HTTP 404 si no se encuentra el registro
                            return StatusCode(404, "No se encuentra el registro");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Devolución de una respuesta HTTP 500 en caso de error, con detalles del error
            return StatusCode(500, "No se puede realizar la petición por: " + ex);
        }
    }

    // Método para manejar solicitudes POST para crear un nuevo médico
    [HttpPost]
    public async Task<IActionResult> Crearmedico([FromBody] Medico medico)
    {
        try
        {
            // Uso de "using" para garantizar la liberación de recursos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta SQL para insertar un nuevo médico
                string query = "INSERT INTO Medico (NombreMed, ApellidoMed, RunMed, Eunacom, NacionalidadMed, Especialidad, horarios, TarifaHr) VALUES (@NombreMed, @ApellidoMed, @RunMed, @Eunacom, @NacionalidadMed, @Especialidad, @horarios, @TarifaHr)";

                // Uso de "using" para garantizar la liberación de recursos
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignación de parámetros a partir del objeto "Medico" recibido en el cuerpo de la solicitud
                    command.Parameters.AddWithValue("@NombreMed", medico.NombreMed);
                    command.Parameters.AddWithValue("@ApellidoMed", medico.ApellidoMed);
                    command.Parameters.AddWithValue("@RunMed", medico.RunMed);
                    command.Parameters.AddWithValue("@Eunacom", medico.Eunacom);
                    command.Parameters.AddWithValue("@NacionalidadMed", medico.NacionalidadMed);
                    command.Parameters.AddWithValue("@Especialidad", medico.Especialidad);
                    command.Parameters.AddWithValue("@horarios", medico.horarios);
                    command.Parameters.AddWithValue("@TarifaHr", medico.TarifaHr);

                    // Ejecución de la consulta de inserción
                    await command.ExecuteNonQueryAsync();

                    // Devolución de una respuesta HTTP 201 indicando que el médico fue creado correctamente
                    return StatusCode(201, $"Medico creado correctamente: {medico}");
                }
            }
        }
        catch (Exception ex)
        {
            // Devolución de una respuesta HTTP 500 en caso de error, con detalles del error
            return StatusCode(500, "No se pudo guardar el registro por: " + ex);
        }
    }

    // Método para manejar solicitudes PUT para editar un médico por su ID
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarMedico(int id, [FromBody] Medico medico)
    {
        try
        {
            // Uso de "using" para garantizar la liberación de recursos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta SQL para actualizar un médico por su ID
                string query = "UPDATE Medico SET NombreMed = @NombreMed, ApellidoMed = @ApellidoMed, RunMed = @RunMed, Eunacom = @Eunacom, NacionalidadMed = @NacionalidadMed, Especialidad = @Especialidad, horarios = @horarios, TarifaHr = @TarifaHr WHERE idMedico = @id";

                // Uso de "using" para garantizar la liberación de recursos
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignación de parámetros a partir del objeto "Medico" recibido en el cuerpo de la solicitud
                    command.Parameters.AddWithValue("@NombreMed", medico.NombreMed);
                    command.Parameters.AddWithValue("@ApellidoMed", medico.ApellidoMed);
                    command.Parameters.AddWithValue("@RunMed", medico.RunMed);
                    command.Parameters.AddWithValue("@Eunacom", medico.Eunacom);
                    command.Parameters.AddWithValue("@NacionalidadMed", medico.NacionalidadMed);
                    command.Parameters.AddWithValue("@Especialidad", medico.Especialidad);
                    command.Parameters.AddWithValue("@horarios", medico.horarios);
                    command.Parameters.AddWithValue("@TarifaHr", medico.TarifaHr);
                    command.Parameters.AddWithValue("@id", id);

                    // Ejecución de la consulta de actualización
                    await command.ExecuteNonQueryAsync();

                    // Devolución de una respuesta HTTP 200 indicando que el registro fue editado correctamente
                    return StatusCode(200, "Registro editado correctamente");
                }
            }
        }
        catch (Exception ex)
        {
            // Devolución de una respuesta HTTP 500 en caso de error, con detalles del error
            return StatusCode(500, "No se pudo editar el medico por: " + ex);
        }
    }

    // Método para manejar solicitudes DELETE para eliminar un médico por su ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarMedico(int id)
    {
        try
        {
            // Uso de "using" para garantizar la liberación de recursos
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta SQL para eliminar un médico por su ID
                string query = "DELETE FROM Medicos WHERE idMedico = @id";

                // Uso de "using" para garantizar la liberación de recursos
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignación de parámetros
                    command.Parameters.AddWithValue("@id", id);

                    // Ejecución de la consulta de eliminación
                    var borrado = await command.ExecuteNonQueryAsync();

                    // Verificación de si se eliminó algún registro
                    if (borrado == 0)
                    {
                        // Devolución de una respuesta HTTP 404 si no se encuentra el registro
                        return StatusCode(404, "Registro no encontrado!!!");
                    }
                    else
                    {
                        // Devolución de una respuesta HTTP 200 indicando que el médico fue eliminado correctamente
                        return StatusCode(200, $"Medico con el ID {id} eliminado correctamente");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Devolución de una respuesta HTTP 500 en caso de error, con detalles del error
            return StatusCode(500, "No se pudo eliminar el registro por: " + ex);
        }
    }
}
