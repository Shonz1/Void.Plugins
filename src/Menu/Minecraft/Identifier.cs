namespace Menu.Minecraft;

public record Identifier(string Value, string Namespace = "minecraft")
{
  public static Identifier FromString(string value)
  {
    var parts = value.Split(':');
    if (parts.Length == 1)
      return new Identifier(parts[0]);

    if (parts.Length == 2)
      return new Identifier(parts[1], parts[0]);

    throw new ArgumentException($"Invalid identifier format: {value}");
  }

  public override string ToString()
  {
    return $"{Namespace}:{Value}";
  }

  public static implicit operator string(Identifier value) => value.ToString();
  public static implicit operator Identifier(string value) => FromString(value);
}
