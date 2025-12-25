namespace ShiftCraft.Mobile.Services;

/// <summary>
/// Centralized API client for all HTTP operations.
/// Single source of truth for base URL, authentication, and error handling.
/// v1.1 - Technical debt elimination
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Performs a GET request to the specified endpoint.
    /// </summary>
    /// <typeparam name="T">Expected response type</typeparam>
    /// <param name="endpoint">Relative endpoint (e.g., "employee", "user/1")</param>
    /// <returns>Deserialized response or default(T) on failure</returns>
    Task<T?> GetAsync<T>(string endpoint);

    /// <summary>
    /// Performs a POST request with JSON body.
    /// </summary>
    /// <typeparam name="T">Expected response type</typeparam>
    /// <param name="endpoint">Relative endpoint</param>
    /// <param name="data">Request body (will be serialized to JSON)</param>
    /// <returns>Deserialized response</returns>
    Task<T?> PostAsync<T>(string endpoint, object? data = null);

    /// <summary>
    /// Performs a PUT request with JSON body.
    /// </summary>
    /// <typeparam name="T">Expected response type</typeparam>
    /// <param name="endpoint">Relative endpoint</param>
    /// <param name="data">Request body</param>
    /// <returns>Deserialized response</returns>
    Task<T?> PutAsync<T>(string endpoint, object data);

    /// <summary>
    /// Performs a DELETE request.
    /// </summary>
    /// <param name="endpoint">Relative endpoint</param>
    /// <returns>True if successful</returns>
    Task<bool> DeleteAsync(string endpoint);

    /// <summary>
    /// Performs a POST request without expecting a response body.
    /// </summary>
    /// <param name="endpoint">Relative endpoint</param>
    /// <param name="data">Optional request body</param>
    /// <returns>True if successful</returns>
    Task<bool> PostAsync(string endpoint, object? data = null);

    /// <summary>
    /// Performs a PUT request without expecting a response body.
    /// </summary>
    /// <param name="endpoint">Relative endpoint</param>
    /// <param name="data">Request body</param>
    /// <returns>True if successful</returns>
    Task<bool> PutAsync(string endpoint, object data);
}
