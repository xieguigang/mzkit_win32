Imports System.ComponentModel
Imports std = System.Math

''' <summary>
''' Histogram control.
''' </summary>
''' 
''' <remarks><para>The control displays histograms represented with integer arrays,
''' where each array's element keeps occurrence number of the corresponding element.
''' </para>
''' 
''' <para>Sample usage:</para>
''' <code>
''' // create array with histogram values
''' int[] histogramValues = new int[] { 3, 8, 53, 57, 79, 69, ... };
''' // set values to histogram control
''' histogram.Values = histogramValues;
''' </code>
''' </remarks>
Public Class Histogram
    Inherits Control
    ' color used to paing histogram
    Private colorField As Color = Color.Black
    ' logarithmic view or not
    Private logarithmic As Boolean = False
    ' histogram's values
    Private valuesField As Integer()
    ' max histogram's values
    Private max As Integer
    Private maxLogarithmic As Double
    ' allow mouse selection in histogram or not
    Private allowSelectionField As Boolean = False
    ' vertical histogram or not
    Private vertical As Boolean = False

    ' set of pens
    Private blackPen As Pen = New Pen(Color.Black, 1)
    Private whitePen As Pen = New Pen(Color.White, 1)
    Private drawPen As Pen = New Pen(Color.Black)

    ' width and height of histogram's area
    Private m_width As Integer
    Private m_height As Integer

    ' mouse dragging with pressed button
    Private tracking As Boolean = False
    ' determines if mouse is over histogram area
    Private over As Boolean = False
    ' selection's start and stop positions
    Private start, [stop] As Integer

    ''' <summary>
    ''' Histogram's color.
    ''' </summary>
    ''' 
    <DefaultValue(GetType(Color), "Black")>
    Public Property Color As Color
        Get
            Return colorField
        End Get
        Set(value As Color)
            colorField = value

            drawPen.Dispose()
            drawPen = New Pen(colorField)
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Allow mouse selection or not.
    ''' </summary>
    ''' 
    ''' <remarks>In the case if mouse selection is allowed, the control will
    ''' fire <see cref="SelectionChanged"/> and <see cref="PositionChanged"/> events
    ''' and provide information about the selection.</remarks>
    ''' 
    <DefaultValue(False)>
    Public Property AllowSelection As Boolean
        Get
            Return allowSelectionField
        End Get
        Set(value As Boolean)
            allowSelectionField = value
        End Set
    End Property

    ''' <summary>
    ''' Logarithmic view or not.
    ''' </summary>
    ''' 
    ''' <remarks><para>In the case if logarihmic view is selected, then the control
    ''' will display base 10 logarithm of values.</para>
    ''' 
    ''' <para>By default the property is set to <b>false</b> - none logarithmic view.</para></remarks>
    ''' 
    <DefaultValue(False)>
    Public Property IsLogarithmicView As Boolean
        Get
            Return logarithmic
        End Get
        Set(value As Boolean)
            logarithmic = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Vertical view or not.
    ''' </summary>
    '''
    ''' <remarks><para>The property determines if histogram should be displayed vertically or
    ''' not (horizontally).</para>
    ''' 
    ''' <para>By default the property is set to <b>false</b> - horizontal view.</para></remarks>
    '''
    <DefaultValue(False)>
    Public Property IsVertical As Boolean
        Get
            Return vertical
        End Get
        Set(value As Boolean)
            vertical = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Histogram values.
    ''' </summary>
    ''' 
    ''' <remarks>Non-negative histogram values.</remarks>
    ''' 
    ''' <exception cref="ArgumentException">Histogram values should be non-negative.</exception>
    ''' 
    <Browsable(False)>
    Public Property Values As Integer()
        Get
            Return valuesField
        End Get
        Set(value As Integer())
            valuesField = value

            If valuesField IsNot Nothing Then
                ' check values and find maximum
                max = 0
                For Each v In valuesField
                    ' value chould non-negative
                    'if (v < 0)
                    '{
                    '    throw new ArgumentException("Histogram values should be non-negative.");
                    '}

                    If v > max Then
                        max = v
                        maxLogarithmic = Math.Log10(max)
                    End If
                Next
            End If
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Mouse position changed event.
    ''' </summary>
    ''' 
    ''' <remarks>The event is fired only if the <see cref="AllowSelection"/> property is set
    ''' to true. The passed to event handler <see cref="HistogramEventArgs"/> class is initialized
    ''' with <see cref="HistogramEventArgs.Position"/> property only, which is histogram value's
    ''' index pointed by mouse.</remarks>
    ''' 
    Public Event PositionChanged As HistogramEventHandler

    ''' <summary>
    ''' Mouse selection changed event.
    ''' </summary>
    ''' 
    ''' <remarks>The event is fired only if the <see cref="AllowSelection"/> property is set
    ''' to true. The passed to event handler <see cref="HistogramEventArgs"/> class is initialized
    ''' with <see cref="HistogramEventArgs.Min"/> and <see cref="HistogramEventArgs.Max"/> properties
    ''' only, which represent selection range - min and max indexes.</remarks>
    ''' 
    Public Event SelectionChanged As HistogramEventHandler

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Histogram"/> class.
    ''' </summary>
    ''' 
    Public Sub New()
        InitializeComponent()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.DoubleBuffer Or ControlStyles.UserPaint, True)
    End Sub

    ''' <summary>
    ''' Dispose the object.
    ''' </summary>
    ''' 
    ''' <param name="disposing">Indicates if disposing was initiated manually.</param>
    ''' 
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            blackPen.Dispose()
            whitePen.Dispose()
            drawPen.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Init component
    Private Sub InitializeComponent()
        ' 
        ' Histogram
        ' 
        AddHandler MouseUp, New MouseEventHandler(AddressOf Histogram_MouseUp)
        AddHandler MouseMove, New MouseEventHandler(AddressOf Histogram_MouseMove)
        AddHandler MouseLeave, New EventHandler(AddressOf Histogram_MouseLeave)
        AddHandler MouseDown, New MouseEventHandler(AddressOf Histogram_MouseDown)
    End Sub

    ''' <summary>
    ''' Paint the control.
    ''' </summary>
    ''' 
    ''' <param name="pe">Data for Paint event.</param>
    ''' 
    Protected Overrides Sub OnPaint(pe As PaintEventArgs)
        Dim g = pe.Graphics
        ' drawing area's width and height
        m_width = If(valuesField Is Nothing OrElse vertical = True, ClientRectangle.Width - 2, Math.Min(valuesField.Length, ClientRectangle.Width - 2))

        m_height = If(valuesField Is Nothing OrElse vertical = False, ClientRectangle.Height - 2, Math.Min(valuesField.Length, ClientRectangle.Height - 2))

        Dim x = 1
        Dim y = 1
        Dim value As Integer

        ' draw rectangle around the image
        'g.DrawRectangle(blackPen, x - 1, y - 1, width + 1, height + 1);

        If valuesField IsNot Nothing Then
            Dim start = Math.Min(Me.start, Me.stop)
            Dim [stop] = Math.Max(Me.start, Me.stop)

            If tracking Then
                ' fill region of selection
                Dim brush As Brush = New SolidBrush(Color.FromArgb(92, 92, 92))

                If vertical Then
                    g.FillRectangle(brush, x, y + start, m_width, std.Abs(start - [stop]) + 1)
                Else
                    g.FillRectangle(brush, x + start, y, std.Abs(start - [stop]) + 1, Height)
                End If
                brush.Dispose()
            End If

            If max <> 0 Then
                ' scaling factor
                Dim factor = If(vertical, m_width, Height) / If(logarithmic, maxLogarithmic, max)

                ' draw histogram
                Dim i = 0, len = If(vertical, Height, m_width)

                While i < len
                    If logarithmic Then
                        value = If(valuesField(i) = 0, 0, CInt(Math.Log10(valuesField(i)) * factor))
                    Else
                        value = CInt(valuesField(i) * factor)
                    End If

                    If value <> 0 Then
                        If vertical Then
                            g.DrawLine(If(tracking AndAlso i >= start AndAlso i <= [stop], whitePen, drawPen), New Point(x, y + i), New Point(x + value, y + i))
                        Else
                            g.DrawLine(If(tracking AndAlso i >= start AndAlso i <= [stop], whitePen, drawPen), New Point(x + i, y + Height - 1), New Point(x + i, y + Height - value))
                        End If
                    End If

                    i += 1
                End While
            End If
        End If

        ' Calling the base class OnPaint
        MyBase.OnPaint(pe)
    End Sub

    ' On mouse down
    Private Sub Histogram_MouseDown(sender As Object, e As MouseEventArgs)
        If allowSelectionField AndAlso valuesField IsNot Nothing Then
            Dim x = 1
            Dim y = 1

            If e.X >= x AndAlso e.Y >= y AndAlso e.X < x + m_width AndAlso e.Y < y + Height Then
                ' start selection
                tracking = True
                start = If(vertical, e.Y - y, e.X - x)
                Capture = True
            End If
        End If
    End Sub

    ' On mouse up
    Private Sub Histogram_MouseUp(sender As Object, e As MouseEventArgs)
        If tracking Then
            ' stop selection
            tracking = False
            Capture = False
            Invalidate()
        End If
    End Sub

    ' On mouse move
    Private Sub Histogram_MouseMove(sender As Object, e As MouseEventArgs)
        If allowSelectionField AndAlso valuesField IsNot Nothing Then
            Dim x = 1
            Dim y = 1

            If Not tracking Then
                If e.X >= x AndAlso e.Y >= y AndAlso e.X < x + m_width AndAlso e.Y < y + Height Then
                    over = True

                    ' moving over
                    Cursor = Cursors.Cross

                    ' notify parent
                    RaiseEvent PositionChanged(Me, New HistogramEventArgs(If(vertical, e.Y - y, e.X - x)))
                Else
                    Cursor = Cursors.Default

                    If over Then
                        over = False

                        ' notify parent
                        RaiseEvent PositionChanged(Me, New HistogramEventArgs(-1))
                    End If
                End If
            Else
                ' selecting region
                [stop] = If(vertical, e.Y - y, e.X - x)

                [stop] = Math.Min([stop], If(vertical, Height, m_width) - 1)
                [stop] = Math.Max([stop], 0)

                Invalidate()

                ' notify parent
                RaiseEvent SelectionChanged(Me, New HistogramEventArgs(Math.Min(start, [stop]), Math.Max(start, [stop])))
            End If
        End If
    End Sub

    ' On mouse leave
    Private Sub Histogram_MouseLeave(sender As Object, e As EventArgs)
        If allowSelectionField AndAlso valuesField IsNot Nothing AndAlso Not tracking Then
            ' notify parent
            RaiseEvent PositionChanged(Me, New HistogramEventArgs(-1))
        End If
    End Sub
End Class
''' <summary>
''' Arguments of histogram events.
''' </summary>
Public Class HistogramEventArgs
    Inherits EventArgs
    Private minField, maxField As Integer

    ''' <summary>
    ''' Initializes a new instance of the <see cref="HistogramEventArgs"/> class.
    ''' </summary>
    ''' 
    ''' <param name="pos">Histogram's index under mouse pointer.</param>
    ''' 
    Public Sub New(pos As Integer)
        minField = pos
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="HistogramEventArgs"/> class.
    ''' </summary>
    ''' 
    ''' <param name="min">Min histogram's index in selection.</param>
    ''' <param name="max">Max histogram's index in selection.</param>
    ''' 
    Public Sub New(min As Integer, max As Integer)
        minField = min
        maxField = max
    End Sub

    ''' <summary>
    ''' Min histogram's index in selection.
    ''' </summary>
    Public ReadOnly Property Min As Integer
        Get
            Return minField
        End Get
    End Property

    ''' <summary>
    ''' Max histogram's index in selection.
    ''' </summary>
    Public ReadOnly Property Max As Integer
        Get
            Return maxField
        End Get
    End Property

    ''' <summary>
    ''' Histogram's index under mouse pointer.
    ''' </summary>
    Public ReadOnly Property Position As Integer
        Get
            Return minField
        End Get
    End Property
End Class
''' <summary>
''' Delegate for histogram events handlers.
''' </summary>
''' <param name="sender">Sender object.</param>
''' <param name="e">Event arguments.</param>
Public Delegate Sub HistogramEventHandler(sender As Object, e As HistogramEventArgs)

