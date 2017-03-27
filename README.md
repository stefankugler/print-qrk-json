# QRK Registrierkasse als Serverdienst für alte Kassenlösungen
Die Software *[QRK Registrierkasse](http://www.ckvsoft.at/)* ist eine einfache Kasse, die nach Angaben der Projekthomepage der österreichischen Registrierkassensicherheitsverordnung entspricht.
Neben der typischen Kassenfunktion nimmt die Kasse im Server Modus Rechnungsdaten per JSON-Datei entgegen. Mit dieser Funktion können Rechnungen aus anderen Programmen heraus erstellt werden.
Hier wird beschrieben, wie Bons aus POS-Software der Codebasis POSper, Chromis POS, uniCenta POS,... mit Hilfe von QRK erstellt und signiert werden können.

Die Beschreibung wurde unter Windows XP und 7 mit der POS-Software POSper getestet. Für Linux gibt es einen Lösungsansatz für Chromis. Rückmeldung zu anderen Kassenlösungen arbeite ich gerne ein!

**Haftungsausschluss:** Der Einsatz der genannten Softwareprodukte erfolgt auf eigenes Risiko!

# Linux
Für Chromis:
- Template Printer.Ticket mit (Printer.Ticket.chromis) ersetzen
- Drucker 1 konfigurieren: Typ: `Epson`, Modus: `file`, Port: `/opt/chromis.pipe`
- Pipe anlegen: `mkfifo /opt/chromis.pipe`
- Skript [bon2json.sh](bon2json.sh) anlegen und ausführbar machen: `chmod a+x bon2json.sh`
- Skript ausführen und QRK starten
- ggf. Berechtigungen anpassen (Pipe für User lesbar machen)
- Todo:
  - Bondruck auf zusätzlichen Drucker setzen (2, 3,...), und Bondrucker als Drucker 1 einrichten, damit können auch alle anderen Bons gedruckt werden: Änderung im Template z.B. `<ticket printer="2">`
  - ESC-Code für Lade in Skript einbauen

# Windows
## QRK Registrierkasse
Siehe [Knowledgebase](http://www.ckvsoft.at/kb/) auf der Projekthomepage. 

Einstellungen:
- Importpfad für den Server-Modus: `C:\CkvSoft\import`
- Server-Modus muss aktiviert sein
- Test: Kopiere die JSON-Testdatei [testbon.json](testbon.json) in den Importpfad

## Installation des MFILEMON-Druckeranschlusses
Die Software *[Multi File Port Monitor](https://sourceforge.net/projects/mfilemon/)* legt einen speziellen Druckeranschluss an und erlaubt die Manipulation von Druckaufträgen. 
- Installiere die Software
- Lege einen neuen Drucker an, wähle *Einen lokalen Drucker hinzufügen* und erstelle einen neuen Anschluss vom Typ *Multi File Port Monitor*.
  - Name des Ports: `kasse`
  - Output Path: `C:\CkvSoft\import`
  - Filename Pattern: `bon-%0000i.txt`. Alternativ `%Y%m%d%H%n%s.txt` für eine Benennung mit Zeitstempel. Weitere Variablen sind in der Port-Konfiguration von des Druckers über die Schaltfläche `?` zu finden.
  - Overwrite existing files: `aktiviert`
  - User command: `C:\CkvSoft\import\removeesc.exe %f`
- Druckertreiber: HP LaserJet 4100 PCL6
- Name des Druckers: `kasse`

## Anpassung des Bon-Layouts in der POS-Software
Das Format der JSON-Datei wurde im [Forum](http://www.ckvsoft.at/forum/qrk-fragen-und-antworten/anbindung-an-boniersystem/#post-648) veröffentlicht. Die Anpassung der Bons der genannten Kassensysteme erfolgt über die Template-Engine *Apache Velocity*. Die Vorlage `Printer.Ticket[.pos-software]` erstellt ein JSON-Objekt für QRK.

**TODO**: Einarbeitung der Zahlungsarten Bankomatkarte/Kreditkarte ins Template

## Einstellung des POS-Druckers
Die POS-Software muss nun dazu gebracht werden, den Bon über den eben erstellten Drucker auszugeben.

POSper:
- Drucker: `seiko`, Modus `rawprinter`, Port: `kasse`; Der Treiber für Seiko-Drucker fügt nur zu Beginn und am Ende der Ausgabedatei Steuerzeichen hinzu. Diese können leicht entfernt werden.

## Entfernen überflüssiger Steuerzeichen
Das C#-Programm `removeesc.exe` (Quellcode: Program.cs, mit SharpDevelop kompiliert) entfernt aus der per Parameter übergebenen Datei alle führenden und abschließenden Steuerzeichen und speichert die Datei mit der Endung `.json` im Importverzeichnis von QRK ab. Sobald QRK in diesem Verzeichnis eine neue Datei entdeckt, wird ein Bon erstellt und gedruckt.

Das Öffnen der Kassenlade muss der Druckertreiber des in QRK konfigurierten Bondruckers übernehmen, sofern notwendig.

### Gescheiterte Versuche
- Nutzung des file-Druckertreibers der POS-Software: Die Ausgabedatei wird während der Laufzeit der POS-Software offen gehalten und neue Ausgaben werden am Ende angehängt. Ein Abgriff der einzelnen JSON-Daten wäre wieder nur mit Zusatzsoftware möglich.
- Verwendung des Windows-Standarddruckers mit dem Druckertreiber "Generic / Text only" und Umgehung der Steuerzeichen-Bereinigung: Die Ausgabedatei enthält nur ein einzelnes Zeichen. Stellt man den Druckertreiber auf z.B. XPS oder PDF (per PDFCreator) um, wird der JSON-Code kopierbar ausgegeben.
- Die genannten Software-Produkte sind freie Software und können ja frei verändert werden: Dazu fehlt die Zeit zur Einarbeitung in den Code und vermutlich reichen auch meine Programmierkenntnisse nicht aus - eine Einreichung des Features *JSON-Export pro Bon* kann an die Entwickler/Community weitergereicht werden.
