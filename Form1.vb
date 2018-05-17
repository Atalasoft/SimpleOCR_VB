' Simple OCR Demo
'*
'*
'* Please refer to the DotImage OCR Documentation for more understanding.
'*
'*


Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports System.IO

Imports Atalasoft.Ocr
Imports Atalasoft.Imaging
Imports Atalasoft.Ocr.GlyphReader
Imports Atalasoft.Ocr.Tesseract
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Codec.Pdf
Imports Atalasoft.Ocr.Abbyy

Namespace SimpleOCR
    ''' <summary>
    ''' Summary description for Form1.
    ''' </summary>
    Public Class Form1 : Inherits System.Windows.Forms.Form
        Const APPTITLE As String = "Simple OCR Demo"
        Private _validLicense, _hasGR As Boolean
        Private Shared _tempDir As String = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "Atalasoft\Demos\OCR_temp"
        Private Shared _tempFile As String = _tempDir & "\temp"
        Private Shared _outputFile As String = _tempDir & "\output.txt"
        Private _selectedMimeType As String = ""
        Private _fileLoaded As Boolean

        Private _engine As OcrEngine
        Private _abbyyEngine As AbbyyEngine
        Private _abbyyLoader As AbbyyLoader

        Private _saveToFile As Boolean = False

        ' event handlers
        Private OnMimeClick As System.EventHandler = Nothing
        'private EventHandler PageProgressHandler = null;

        Private textBox1 As System.Windows.Forms.TextBox
        Private mainMenu1 As System.Windows.Forms.MainMenu
        Private workspaceViewer1 As Atalasoft.Imaging.WinControls.WorkspaceViewer
        Private openFileDialog1 As System.Windows.Forms.OpenFileDialog
        Private menuAction As System.Windows.Forms.MenuItem
        Private progressBar1 As System.Windows.Forms.ProgressBar
        Private menuFile As System.Windows.Forms.MenuItem
        Private WithEvents menuFileOpen As System.Windows.Forms.MenuItem
        Private WithEvents menuFileExit As System.Windows.Forms.MenuItem
        Private menuHelp As System.Windows.Forms.MenuItem
        Private WithEvents menuHelpAbout As System.Windows.Forms.MenuItem
        Private WithEvents menuActionResult As System.Windows.Forms.MenuItem
        Private WithEvents menuActionTranslate As System.Windows.Forms.MenuItem
        Private WithEvents menuActionSave As System.Windows.Forms.MenuItem
        Private WithEvents menuActionDisplay As System.Windows.Forms.MenuItem
        Private menuItem1 As System.Windows.Forms.MenuItem
        Private WithEvents menuGlyphReaderEngine As System.Windows.Forms.MenuItem
        Private WithEvents menuExperVisionEngine As System.Windows.Forms.MenuItem
        Private WithEvents splitter1 As System.Windows.Forms.Splitter
        Private WithEvents menuTesseract As System.Windows.Forms.MenuItem
        Friend WithEvents menuAbbyy As System.Windows.Forms.MenuItem
        Friend WithEvents menuTesseract3 As System.Windows.Forms.MenuItem
        Private components As System.ComponentModel.IContainer


#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()> _
        Shared Sub Main()
            'this is required in the static method to load GlyphReader resources
            Dim loader As GlyphReaderLoader = New GlyphReaderLoader
            Application.Run(New Form1)
        End Sub


        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not components Is Nothing Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub


        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.textBox1 = New System.Windows.Forms.TextBox
            Me.mainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
            Me.menuFile = New System.Windows.Forms.MenuItem
            Me.menuFileOpen = New System.Windows.Forms.MenuItem
            Me.menuFileExit = New System.Windows.Forms.MenuItem
            Me.menuAction = New System.Windows.Forms.MenuItem
            Me.menuActionResult = New System.Windows.Forms.MenuItem
            Me.menuActionDisplay = New System.Windows.Forms.MenuItem
            Me.menuActionSave = New System.Windows.Forms.MenuItem
            Me.menuActionTranslate = New System.Windows.Forms.MenuItem
            Me.menuItem1 = New System.Windows.Forms.MenuItem
            Me.menuGlyphReaderEngine = New System.Windows.Forms.MenuItem
            Me.menuAbbyy = New System.Windows.Forms.MenuItem
            Me.menuTesseract = New System.Windows.Forms.MenuItem
            Me.menuTesseract3 = New System.Windows.Forms.MenuItem
            Me.menuHelp = New System.Windows.Forms.MenuItem
            Me.menuHelpAbout = New System.Windows.Forms.MenuItem
            Me.workspaceViewer1 = New Atalasoft.Imaging.WinControls.WorkspaceViewer
            Me.openFileDialog1 = New System.Windows.Forms.OpenFileDialog
            Me.progressBar1 = New System.Windows.Forms.ProgressBar
            Me.splitter1 = New System.Windows.Forms.Splitter
            Me.SuspendLayout()
            '
            'textBox1
            '
            Me.textBox1.Dock = System.Windows.Forms.DockStyle.Left
            Me.textBox1.Location = New System.Drawing.Point(0, 0)
            Me.textBox1.Multiline = True
            Me.textBox1.Name = "textBox1"
            Me.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.textBox1.Size = New System.Drawing.Size(336, 649)
            Me.textBox1.TabIndex = 2
            '
            'mainMenu1
            '
            Me.mainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFile, Me.menuAction, Me.menuItem1, Me.menuHelp})
            '
            'menuFile
            '
            Me.menuFile.Index = 0
            Me.menuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFileOpen, Me.menuFileExit})
            Me.menuFile.Text = "File"
            '
            'menuFileOpen
            '
            Me.menuFileOpen.Index = 0
            Me.menuFileOpen.Text = "Open"
            '
            'menuFileExit
            '
            Me.menuFileExit.Index = 1
            Me.menuFileExit.Text = "Exit"
            '
            'menuAction
            '
            Me.menuAction.Index = 1
            Me.menuAction.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuActionResult, Me.menuActionTranslate})
            Me.menuAction.Text = "Action"
            '
            'menuActionResult
            '
            Me.menuActionResult.Index = 0
            Me.menuActionResult.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuActionDisplay, Me.menuActionSave})
            Me.menuActionResult.Text = "Result"
            '
            'menuActionDisplay
            '
            Me.menuActionDisplay.Checked = True
            Me.menuActionDisplay.Index = 0
            Me.menuActionDisplay.RadioCheck = True
            Me.menuActionDisplay.Text = "Displays in Text Box"
            '
            'menuActionSave
            '
            Me.menuActionSave.Index = 1
            Me.menuActionSave.RadioCheck = True
            Me.menuActionSave.Text = "Saves to File"
            '
            'menuActionTranslate
            '
            Me.menuActionTranslate.Index = 1
            Me.menuActionTranslate.Text = "Translate ..."
            '
            'menuItem1
            '
            Me.menuItem1.Index = 2
            Me.menuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuGlyphReaderEngine, Me.menuAbbyy, Me.menuTesseract, Me.menuTesseract3})
            Me.menuItem1.Text = "Engine"
            '
            'menuGlyphReaderEngine
            '
            Me.menuGlyphReaderEngine.Index = 0
            Me.menuGlyphReaderEngine.Text = "GlyphReader Engine"
            '
            'menuAbbyy
            '
            Me.menuAbbyy.Index = 1
            Me.menuAbbyy.Text = "Abbyy"
            '
            'menuTesseract
            '
            Me.menuTesseract.Index = 2
            Me.menuTesseract.Text = "Tesseract Engine"
            '
            'menuTesseract3
            '
            Me.menuTesseract3.Index = 3
            Me.menuTesseract3.Text = "Tesseract3 Engine"
            '
            'menuHelp
            '
            Me.menuHelp.Index = 3
            Me.menuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuHelpAbout})
            Me.menuHelp.Text = "Help"
            '
            'menuHelpAbout
            '
            Me.menuHelpAbout.Index = 0
            Me.menuHelpAbout.Text = "About ..."
            '
            'workspaceViewer1
            '
            Me.workspaceViewer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.workspaceViewer1.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.ReductionOnly
            Me.workspaceViewer1.AutoZoom = Atalasoft.Imaging.WinControls.AutoZoomMode.BestFitShrinkOnly
            Me.workspaceViewer1.DisplayProfile = Nothing
            Me.workspaceViewer1.Location = New System.Drawing.Point(352, 0)
            Me.workspaceViewer1.Magnifier.BackColor = System.Drawing.Color.White
            Me.workspaceViewer1.Magnifier.BorderColor = System.Drawing.Color.Black
            Me.workspaceViewer1.Magnifier.Size = New System.Drawing.Size(100, 100)
            Me.workspaceViewer1.Name = "workspaceViewer1"
            Me.workspaceViewer1.OutputProfile = Nothing
            Me.workspaceViewer1.Selection = Nothing
            Me.workspaceViewer1.Size = New System.Drawing.Size(520, 616)
            Me.workspaceViewer1.TabIndex = 3
            Me.workspaceViewer1.Text = "workspaceViewer1"
            '
            'progressBar1
            '
            Me.progressBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.progressBar1.Location = New System.Drawing.Point(344, 624)
            Me.progressBar1.Name = "progressBar1"
            Me.progressBar1.Size = New System.Drawing.Size(528, 24)
            Me.progressBar1.TabIndex = 4
            '
            'splitter1
            '
            Me.splitter1.Location = New System.Drawing.Point(336, 0)
            Me.splitter1.Name = "splitter1"
            Me.splitter1.Size = New System.Drawing.Size(8, 649)
            Me.splitter1.TabIndex = 5
            Me.splitter1.TabStop = False
            '
            'Form1
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(880, 649)
            Me.Controls.Add(Me.splitter1)
            Me.Controls.Add(Me.progressBar1)
            Me.Controls.Add(Me.workspaceViewer1)
            Me.Controls.Add(Me.textBox1)
            Me.Menu = Me.mainMenu1
            Me.Name = "Form1"
            Me.Text = "Atalasoft Simple OCR Demo"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

        Shared Sub New()
            AtalaDemos.HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders)
        End Sub

        Public Sub New()
            ' Verify the DotImage license.
            CheckLicenseFile()

            If _validLicense Then
                '
                ' Required for Windows Form Designer support
                '
                InitializeComponent()

                ' add event handler for mime type list
                Me.OnMimeClick = New System.EventHandler(AddressOf Me.OnMimeClick1)

                ' Pick a licensed engine to start with.
                Me.menuGlyphReaderEngine.Enabled = _hasGR
                If _hasGR Then
                    MakeGlyphReaderEngine()
                    Me.menuGlyphReaderEngine.Checked = True
                Else
                    MakeTesseractEngine()
                    Me.menuTesseract.Checked = True
                End If

            End If
        End Sub



#Region "Check for license code"


        Private Sub CheckGRLicense()
            Try
                _hasGR = True
                Dim gr As GlyphReaderEngine = New GlyphReaderEngine   ' does not throw
                gr.Initialize() ' will throw on no license
                gr.Dispose()
            Catch e1 As AtalasoftLicenseException
                _hasGR = False
            End Try
        End Sub

        Private Sub CheckLicenseFile()
            ' Make sure a license for DotImage and Advanced DocClean exist.
            Try
                Dim img As AtalaImage = New AtalaImage
                img.Dispose()
            Catch ex1 As Atalasoft.Imaging.AtalasoftLicenseException
                LicenseCheckFailure(ex1.Message)
                Return
            End Try

            If AtalaImage.Edition <> LicenseEdition.Document Then
                LicenseCheckFailure("This demo requires a Document Imaging License." & Constants.vbCrLf & "Your current license is for '" & AtalaImage.Edition.ToString() & "'.")
                Return
            End If

            Try
                Dim t As TranslatorCollection = New TranslatorCollection
            Catch e1 As AtalasoftLicenseException
                LicenseCheckFailure("This demo requires an OCR license.")
                Return
            End Try

            CheckGRLicense()
            Me._validLicense = True
        End Sub

        Private Sub LicenseCheckFailure(ByVal message As String)
            AddHandler Load, AddressOf Form1_Load
            If MessageBox.Show(Me, message & Constants.vbCrLf & Constants.vbCrLf & "Would you like to request an evaluation license?", "License Required", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then
                Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.Load("Atalasoft.dotImage")
                If Not asm Is Nothing Then
                    Dim version As String = asm.GetName().Version.ToString(2)

                    ' Locate the activation utility.
                    Dim path As String = ""
                    Dim key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\Atalasoft\dotImage\" & version)
                    If Not key Is Nothing Then
                        path = Convert.ToString(key.GetValue("AssemblyBasePath"))
                        If Not path Is Nothing AndAlso path.Length > 5 Then
                            path = path.Substring(0, path.Length - 3) & "AtalasoftToolkitActivation.exe"
                        Else
                            path = System.IO.Path.GetFullPath("..\..\..\..\..\AtalasoftToolkitActivation.exe")
                        End If

                        key.Close()
                    End If

                    If System.IO.File.Exists(path) Then
                        System.Diagnostics.Process.Start(path)
                    Else
                        MessageBox.Show(Me, "We were unable to location the DotImage activation utility." & Constants.vbCrLf & "Please run it from the Start menu shortcut.", "File Not Found")
                    End If
                Else
                    MessageBox.Show(Me, "Unable to load the DotImage assembly.", "Load Error")
                End If
            End If
        End Sub

        '		private void Form1_Load(object sender, System.EventArgs e)
        '		{
        '			// close the demo if there is no valid license
        '			if (!this._validLicense)
        '				Application.Exit();
        '		}

#End Region

#Region "Load Mime Types"
        ' event handler to apply find the selected mime type
        Private Sub OnMimeClick1(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim item As MenuItem = CType(sender, MenuItem)
            ' save for using in OCR translate
            _selectedMimeType = item.Text
            ' This is the submenu to "Translate ..." so we want to start translation to display only, here.
            DoTranslation()
        End Sub

        ' load all of the supported mime types into a menu.
        Public Sub LoadMimeMenu()
            Me.menuActionTranslate.MenuItems.Clear()

            ' add each type
            Dim mimes As String() = _engine.SupportedMimeTypes()
            For Each s As String In mimes
                Me.menuActionTranslate.MenuItems.Add(s, Me.OnMimeClick)
            Next s
            ' first entry is default
            ' save for using in OCR translate
            _selectedMimeType = Me.menuActionTranslate.MenuItems(0).Text

        End Sub

#End Region

#Region "File Menu Events"
        ' This method copies the selected file into a temp directory for OCR processing.
        ' The file must be coppied because the Translate method must be supplied a directory
        ' containing all of the images to process.
        Private Sub menuFileOpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileOpen.Click
            ' try to locate images folder
            Dim imagesFolder As String = Application.ExecutablePath
            ' we assume we are running under the DotImage install folder
            Dim pos As Integer = imagesFolder.IndexOf("DotImage ")
            If pos <> -1 Then
                imagesFolder = imagesFolder.Substring(0, imagesFolder.IndexOf("\", pos)) & "\Images\Documents"
            End If

            'use this folder as starting point			
            Me.openFileDialog1.InitialDirectory = imagesFolder
            ' Add a dialog filter that allows all registered types to open
            Me.openFileDialog1.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(True)

            If Me.openFileDialog1.ShowDialog() = DialogResult.OK Then
                If (Not Directory.Exists(_tempDir)) Then
                    Directory.CreateDirectory(_tempDir)
                End If

                File.Copy(Me.openFileDialog1.FileName, _tempFile, True)

                Try
                    ' display the file.
                    If Me.workspaceViewer1.Image IsNot Nothing Then
                        Me.workspaceViewer1.Image.Dispose()
                    End If

                    Me.workspaceViewer1.Image = New AtalaImage(_tempFile)
                    Me._fileLoaded = True
                Catch Exception As Exception
                    Me._fileLoaded = False
                    MessageBox.Show("Unable to open requested image... Unsupported Image Type.")
                End Try
            End If
        End Sub

        Private Sub Form1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
            ' ShutDown only when the form is being closed.
            If Not _engine Is Nothing Then
                _engine.ShutDown()
            End If
        End Sub

        Private Sub menuFileExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileExit.Click
            Application.Exit()
        End Sub
        ' user picks a file name and mime type.
        Private Function GetSaveFileName() As String
            Dim saveFile As SaveFileDialog = New SaveFileDialog
            Dim mimeTypes As String() = _engine.SupportedMimeTypes()
            saveFile.Filter = PopulateFilter(mimeTypes)
            saveFile.AddExtension = True
            'select the correct starting filter index
            Dim i As Integer = 0
            'ORIGINAL LINE: for (int i = 0; i < mimeTypes.Length; i += 1)
            'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
            Do While i < mimeTypes.Length
                If mimeTypes(i) = _selectedMimeType Then
                    saveFile.FilterIndex = i + 1
                    Exit Do
                End If
                i += 1
            Loop
            If saveFile.ShowDialog() = DialogResult.OK Then
                ' find out which mime type was selected, and save this for use when translating
                _selectedMimeType = mimeTypes(saveFile.FilterIndex - 1)
                Return saveFile.FileName
            End If
            Me.Refresh()
            Return Nothing
        End Function

        ' returns a string formatted for use as a filter in save/open FileDialog populated
        ' with all the supported mime types.  An important thing is that the foreach statement
        ' goes trough the array in assending order or else we will now know which mime type is
        ' selected.
        Private Function PopulateFilter(ByVal types As String()) As String
            Dim mimeFilter As String = ""
            For Each s As String In types
                Select Case s
                    Case "text/plain"
                        mimeFilter &= s & " (.txt)|*.txt|"
                    Case "text/html"
                        mimeFilter &= s & " (.htm,.html)|*.htm;*.html|"
                    Case "text/richtext"
                        mimeFilter &= s & " (.rtf)|*.rtf|"
                    Case "image/x-amidraw"
                        mimeFilter &= s & " (.txt)|*.txt|"
                    Case "application/pdf"
                        mimeFilter &= s & " (.pdf)|*.pdf|"
                    Case "application/msword"
                        mimeFilter &= s & " (.doc)|*.doc|"
                    Case "application/wordperfect"
                        mimeFilter &= s & " (.wpd)|*.wpd|"
                    Case "text/tab-separated-values"
                        mimeFilter &= s & " (.txt)|*.txt|"
                    Case "text/csv"
                        mimeFilter &= s & " (.csv)|*.csv|"
                    Case "text/comma-separated-values"
                        mimeFilter &= s & " (.csv)|*.csv|"
                    Case "application/vnd.lotus-1-2-3"
                        mimeFilter &= s & " (.txt)|*.txt|"
                    Case Else
                        mimeFilter &= s & " (.???)|*.*|"
                End Select
            Next s
            ' remove the last '|'
            Return mimeFilter.Remove(mimeFilter.Length - 1, 1)
        End Function
#End Region

#Region "OCR"
        Private Sub MakeGlyphReaderEngine()
            Dim oldEngine As OcrEngine = _engine
            _engine = New Atalasoft.Ocr.GlyphReader.GlyphReaderEngine
            If Not oldEngine Is Nothing Then
                oldEngine.ShutDown()
            End If
            TieInEngineEvents()
        End Sub
        Private Sub MakeTesseractEngine()
            Dim oldEngine As OcrEngine = _engine
            _engine = New Atalasoft.Ocr.Tesseract.TesseractEngine
            If Not oldEngine Is Nothing Then
                oldEngine.ShutDown()
            End If
            TieInEngineEvents()
        End Sub
        Private Sub MakeTesseract3Engine()
            Dim oldEngine As OcrEngine = _engine
            _engine = New Atalasoft.Ocr.Tesseract.Tesseract3Engine
            If Not oldEngine Is Nothing Then
                oldEngine.ShutDown()
            End If
            TieInEngineEvents()
        End Sub
        Private Sub TieInEngineEvents()
            ' Add event handler to show translation progress
            AddHandler _engine.PageProgress, AddressOf _engine_PageProgress
            _engine.Initialize()
            AddPdfTranslator()
            ' Adds the list of supported output formats to the 'Action' menu.
            LoadMimeMenu()
        End Sub
        Private Sub AddPdfTranslator()
            Dim pdf As PdfTranslator = New PdfTranslator
            pdf.OutputType = PdfTranslatorOutputType.TextUnderImage
            _engine.Translators.Add(pdf)
        End Sub

        ' event handler to select what to do with the results
        Private Sub menuActionOcr_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuActionResult.Click, menuActionDisplay.Click, menuActionSave.Click
            Dim item As MenuItem = CType(sender, MenuItem)
            If item.Checked Then
                Return
            End If

            If item.Index = 1 Then
                _saveToFile = True
                menuActionSave.Checked = True
                menuActionDisplay.Checked = False
            Else
                _saveToFile = False
                menuActionDisplay.Checked = True
                menuActionSave.Checked = False
                LoadMimeMenu()
            End If
        End Sub

        ' this eventhandler will show the progress of reading each page.
        Private Sub _engine_PageProgress(ByVal sender As Object, ByVal e As OcrPageProgressEventArgs)
            Me.progressBar1.Value = e.Progress
        End Sub

        Private Sub menuActionTranslate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuActionTranslate.Click
            ' only act on this event if in save to file mode, otherwise the translation
            ' is started by a sub-menu.
            If _saveToFile Then
                DoTranslation()
            End If
        End Sub

        '  This method does the actual translation into text.
        Private Sub DoTranslation()
            If (Not Me._fileLoaded) Then
                MessageBox.Show("No File Loaded... Please open a file and try again.")
            Else
                Try
                    ' choose output file location, either a temp directory, or a user selected spot.
                    If _saveToFile Then
                        _outputFile = GetSaveFileName()
                    End If
                    If _outputFile Is Nothing Then
                        Return
                    End If

                    ' delete the output file if one already exists
                    If File.Exists(_outputFile) Then
                        File.Delete(_outputFile)
                    End If


                    Me.textBox1.Clear()

                    ' this is how the image should be passed to the translator
                    Dim myIS As FileSystemImageSource = New FileSystemImageSource(_tempDir, True)

                    Me.progressBar1.Value = 0

                    ' do the acctual translation here.  The text is saved as a file in _outputFile.

                    _engine.Translate(myIS, _selectedMimeType, _outputFile)

                    If (Not _saveToFile) Then
                        ' Load the text back into the text box for display.
                        Dim input As StreamReader = New StreamReader(_outputFile)
                        Dim oneLine As String = input.ReadLine()
                        Do While Not oneLine Is Nothing
                            Me.textBox1.AppendText(oneLine)
                            oneLine = input.ReadLine()
                        Loop
                        input.Close()
                    Else
                        System.Diagnostics.Process.Start(_outputFile)
                    End If
                Catch ex As Exception
                    ' if its a license exception, its probably because of pdfTranslator
                    If (TypeOf ex Is AtalasoftLicenseException) AndAlso (_selectedMimeType = "application/pdf") Then
                        MessageBox.Show(Me, "You do not have a license for the PdfTranslator, please request an evaluation.")
                    Else
                        MessageBox.Show(Me, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Try
            End If
        End Sub
#End Region

        Private Sub menuHelpAbout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuHelpAbout.Click
            Dim aboutBox As AtalaDemos.AboutBox.About = New AtalaDemos.AboutBox.About("About Atalasoft Simple OCR Demo", "DotImage Simple OCR Demo")
            aboutBox.Description = "Demonstrates the basics of OCR.  This 'no frills' example demonstrates translating an image to a text file or searchable PDF.  The output text style (or mime type) can be formatted as any of the supported types.  This is a great place to get started with DotImage OCR.  Requires evaluation or purchased licenses of DotImage Document Imaging, and at least one of these OCR Add-ons: GlyphReader, RecoStar or Tesseract."
            aboutBox.ShowDialog()
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
            If (Not Me._validLicense) Then
                Me.Close()
            End If
        End Sub

        Private Sub menuGlyphReaderEngine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuGlyphReaderEngine.Click
            Try
                MakeGlyphReaderEngine()
                menuGlyphReaderEngine.Checked = True
                menuTesseract.Checked = False
            Catch ex As AtalasoftLicenseException
                MessageBox.Show("This Demo requires a DotImage GlyphReader OCR License.  Please request an evaluation license, or purchase one from www.atalasoft.com" & Constants.vbCrLf & Constants.vbCrLf & ex.ToString(), "Atalasoft License Exception", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
            End Try
        End Sub

        Private Sub splitter1_SplitterMoved(ByVal sender As Object, ByVal e As System.Windows.Forms.SplitterEventArgs) Handles splitter1.SplitterMoved
            textBox1.Width = splitter1.Left
            workspaceViewer1.Left = splitter1.Left + splitter1.Width
            workspaceViewer1.Width = Me.ClientSize.Width - workspaceViewer1.Left
            progressBar1.Left = workspaceViewer1.Left
            progressBar1.Width = workspaceViewer1.Width
        End Sub

        Private Sub menuTesseract_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTesseract.Click
            Try
                MakeTesseractEngine()
                menuGlyphReaderEngine.Checked = False
                menuTesseract.Checked = True
                menuTesseract3.Checked = False
                menuAbbyy.Checked = False
            Catch ex As AtalasoftLicenseException
                MessageBox.Show("Selecting Tesseract OCR requires a DotImage OCR License.  Please request an evaluation license, or purchase one from www.atalasoft.com" & Constants.vbCrLf & Constants.vbCrLf & ex.ToString(), "Atalasoft License Exception", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
            End Try

        End Sub

        Private Sub menuTesseract3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuTesseract3.Click
            Try
                MakeTesseract3Engine()
                menuGlyphReaderEngine.Checked = False
                menuTesseract.Checked = False
                menuTesseract3.Checked = True
                menuAbbyy.Checked = False
            Catch ex As AtalasoftLicenseException
                MessageBox.Show("Selecting Tesseract OCR requires a DotImage OCR License.  Please request an evaluation license, or purchase one from www.atalasoft.com" & Constants.vbCrLf & Constants.vbCrLf & ex.ToString(), "Atalasoft License Exception", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
            End Try
        End Sub

        Private Sub menuAbbyy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuAbbyy.Click
            SelectAbbyyEngine()
            menuGlyphReaderEngine.Checked = False
            menuTesseract3.Checked = False
            menuTesseract.Checked = False
            menuAbbyy.Checked = True
        End Sub

        Private Sub SelectAbbyyEngine()
            If _abbyyEngine Is Nothing Then
                Try
                    _abbyyLoader = New AbbyyLoader("C:\Program Files (x86)\Atalasoft\DotImage 10.7\bin\OCRResources\ABBYY")
                    If _abbyyLoader IsNot Nothing Then
                        ' try to create a RecoStar engine
                        _abbyyEngine = New AbbyyEngine()
                        'InitializeEngine(_recoStar)
                        _abbyyEngine.Initialize()
                        AddHandler _abbyyEngine.PageProgress, AddressOf _engine_PageProgress
                        Dim pdf As New PdfTranslator()
                        pdf.OutputType = PdfTranslatorOutputType.TextUnderImage
                        _abbyyEngine.Translators.Add(pdf)
                    End If
                Catch ex As AtalasoftLicenseException
                    LicenseCheckFailure("Using Abbyy OCR requires an Atalasoft DotImage OCR License ... " & ex.Message)
                Catch err As Exception
                    MessageBox.Show(err.Message, APPTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Try
            End If
            If _abbyyEngine IsNot Nothing Then
                'UpdateMenusForEngine()
                Dim oldEngine As OcrEngine = _engine
                _engine = _abbyyEngine
                If Not oldEngine Is Nothing Then
                    oldEngine.ShutDown()
                End If
            End If
        End Sub


    End Class
End Namespace
