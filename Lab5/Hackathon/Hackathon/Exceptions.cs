namespace Hackathon;

public class EmptyFileException(string message) : Exception(message);

public class IncorrectEmployeesDataException(string message) : Exception(message);

public class HrManagerDistributionException(string message) : Exception(message);