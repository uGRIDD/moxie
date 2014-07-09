1 StringBuffer
StringBuffer is a simulated class of Java StringBuffer in javascript, which right now is used in: moxie\src\javascript\runtime\html5\file\FileInput.js

2 zip.js
zip.js is a JavaScript library to zip and unzip files which in our case is to read the structure of zip file.

url:
http://gildas-lormeau.github.io/zip.js/
https://github.com/gildas-lormeau/zip.js
zip.js is used in: moxie\src\javascript\runtime\html5\file\FileInput.js

When using this uGRIDD version of plupload, in the index.html, need to add 
<script type = "text/javascript" src="./lib/zip.js"></script> 
and
<script type = "text/javascript" src="./lib/StringBuffer.js"></script>
where ./lib should be in the same folder with the index.html