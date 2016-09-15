var UglifyJS = require('uglify-js');
var fs = require('fs');

var result = UglifyJS.minify('bundle.js', {
    warnings: true,
    mangle: {
        except: ['checkLoad', 'doIt'],
        //toplevel: false,
        //keep_fnames: true
    },
	compress: {
		sequences: true,
		dead_code: true,
		conditionals: true,
		booleans: true,
		unused: false,
		if_return: true,
		join_vars: true,
		drop_console: true
	}
});

fs.writeFileSync('bundle.min3.js', result.code);