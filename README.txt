# WARNING: THIS PROJECT IS NO LONGER BEING MAINTAINED

### FAST SCHEMATRON FOR .NET
Are you a .NET developer who needs to validate XML files against schematron specs? 
Are you tired of having to carry around with you Saxon DLLs which wrap Java bytecode?
Do you need greater performance than you can get launching a separate process each time you want to check an XML file against the schematron rules to which it must conform?

Then this is the project for you!

### XPATH2/XSLT2
We started this with an excellent ISO schematron authored by the XRouter project in C# (Xpath1 only). Then, we added in the leading .NET library for XPATH2/XSLT2 support -- XmlPrime. Finally, it's being used daily to validate HL7 CCDA and HITSP C32 v2.5 documents against their schematron files. 

### Authors and Contributors
We are grateful to Bohumir Zámecník @bzamecnik and the XRouter team for the ISO Schematron to start with.

### SOURCE CODE
Source code of SchemaTron is available in Git repository hosted at Github.
Browse code: https://github.com/gap777/SchemaTron

### Requirements
- Microsoft Windows 7 32-bit or 64-bit or Microsoft Windows Server 2008
- Microsoft .NET Framework 4.0

#### License
SchemaTron is released under the MIT license. See the LICENSE file for details. Note that the XmlPrime library providing XPath2/XSLT2 support is free for non-commerical applications; otherwise it requires a commercial license.

