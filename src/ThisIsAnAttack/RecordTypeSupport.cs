namespace ThisIsAnAttack;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, Inherited = false)]
internal sealed class IsReadOnlyAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal sealed class IsByRefLikeAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class IsExternalInit : Attribute
{
}
