class PipingSection : IEquatable<PipingSection>
{
    public PipingSection(Element element)
    {
        if (element is Pipe || element is FamilyInstance || element is FlexPipe)
            Element = element;
    }

    public int Section { get; set; }
    public string Name
    {
        get
        {
            return Element.Name;
        }

    }
    public ElementId Id
    {
        get
        {
            return Element.Id;
        }
    }
    public Element Element { get; }
    public PipeDirection Direction { get; set; }

    public override string ToString()
    {
        return $"ID: {Id} / NAME: {Name} / {Direction}";
    }

    public bool IsVertical()
    {
        if (Element is Pipe && GeometryUtility.GetPipeOrientation((Element as Pipe)) == PipeOrientation.VERTICAL)
            return true;
        else
            return false;
    }

    public bool IsSloped()
    {
        if (Element is Pipe && GeometryUtility.GetPipeOrientation((Element as Pipe), 2) == PipeOrientation.SLOPED)
            return true;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return Id.IntegerValue;
    }

    public bool Equals(PipingSection pipingSection)
    {
        if (pipingSection == null)
            return false;
        return (Id.IntegerValue.Equals(pipingSection.Id.IntegerValue));
    }
}