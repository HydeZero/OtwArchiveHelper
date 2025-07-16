# OTWArchiveHelper
## An unofficial helper for the OTW Archive
### THIS PROJECT IS IN NO WAY AFFILIATED WITH THE ORGANIZATION FOR TRANSFORMATIVE WORKS, THE ARCHIVE OF OUR OWN, OR ANY OTHER 3RD PARTY

>[!CAUTION]
> DO NOT USE THIS PROJECT TO BREAK THE TERMS OF SERVICE OF ANY WEBSITES. By using this library, you agree that you understand the risks and that the developer isn't liable for any consequences that result.

This project is just a helper to allow other C# programs to grab content from websites using the [otwarchive](https://github.com/otwcode/otwarchive) as a backend. This project relies on scraping. You can use this to:

* Make an Avalonia UI app to read such content
* Make a terminal program

A demo program is included with this repository.

### Is this legal?
It depends on the site where it's hosted. Please review the ToS of the website before you begin using this tool. 
For AO3, the Terms of Service, specifically I.F. ("What you can't do"), state that you are NOT allowed:
> "...to conduct any commercial activity, whether for direct or indirect commercial advantage, including (without limitation) making available any advertising, spam, or other solicitation, or scraping Content in order to commercialize it;..."

This means that although you CAN scrape content, it has to be 100% non-commercial. As a result, it is HIGHLY advised to avoid putting ads in tools that use this project.

## Known Issues
* Non-Canonical tags crash the program

## Installation/Usage
A NuGet Package will be avaliable soon.

See documentation in [reference.md](./reference.md)

### 3rd Party Libraries

This project contains 3rd Party Libraries licensed under the MIT License.

ReverseMarkdown:
```
The MIT License (MIT)

Copyright (c) 2015 Babu Annamalai

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

HTMLAgilityPack:
```
The MIT License (MIT)
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```