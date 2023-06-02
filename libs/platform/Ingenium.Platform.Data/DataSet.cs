namespace Ingenium.Platform.Security;

/// <summary>
/// Represents a dataset ID.
/// </summary>
[GenerateId(typeof(string), caseInsensitive: true)]
public partial struct DataSetId { }

/// <summary>
/// Represents a dataset.
/// </summary>
public record DataSet(
	DataSetId Id,
	string Name,
	string? Description = default);

/// <summary>
/// Represents a dataset permission.
/// </summary>
/// <param name="DataSet">The dataset.</param>
/// <param name="Create">Flag to state whether this permission is for creating records.</param>
/// <param name="Read">Flag to state whether this permission is for reading records.</param>
/// <param name="Update">Flag to state whether this permission is for updating records.</param>
/// <param name="Delete">Flag to state whether this permission is for deleting records.</param>
public record DataSetPermission(
	DataSet DataSet,
	bool Create,
	bool Read,
	bool Update,
	bool Delete);

/// <summary>
/// Represents the dataset permission types.
/// </summary>
public enum DataSetPermissionType
{
	Create,
	Read,
	Update,
	Delete
}