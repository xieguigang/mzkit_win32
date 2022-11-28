Imports System.Xml.Serialization

Namespace DeepZoomBuilder

    <XmlRoot("Image", [Namespace]:="http://schemas.microsoft.com/deepzoom/2008")>
    <XmlType("Image", [Namespace]:="http://schemas.microsoft.com/deepzoom/2008")>
    Public Class DeepZoomImage

        <XmlAttribute> Public Property TileSize As Integer
        <XmlAttribute> Public Property Overlap As Integer
        <XmlAttribute> Public Property Format As String

        Public Property Size As ImageSize
        Public Property DisplayRects As DisplayRect()

    End Class

    Public Class ImageSize
        <XmlAttribute> Public Property Width As Integer
        <XmlAttribute> Public Property Height As Integer
    End Class

    Public Class DisplayRect

        <XmlAttribute> Public Property MinLevel As Integer
        <XmlAttribute> Public Property MaxLevel As Integer

        Public Property Rect As TileRect

    End Class

    <XmlType("Rect", [Namespace]:="http://schemas.microsoft.com/deepzoom/2008")>
    Public Class TileRect
        <XmlAttribute> Public Property X As Integer
        <XmlAttribute> Public Property Y As Integer
        <XmlAttribute> Public Property Width As Integer
        <XmlAttribute> Public Property Height As Integer
    End Class
End Namespace