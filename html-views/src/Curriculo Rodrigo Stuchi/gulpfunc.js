import gulpLoadPlugins from 'gulp-load-plugins';
import yargs from 'yargs';
import chalk from 'chalk';
import del from 'del';
import fs from 'fs'; //node dependency

const through = require('through2');
const $ = gulpLoadPlugins({
    lazy: true,
    rename: {
        'gulp-inject-string': 'injstr'
    }
});
const args = yargs.argv;

module.exports = {
    UglifyConfig: $.uglify({
        //warnings: true,
        mangle: {
            except: ['exp.checkPageLoad', 'exp.decrypt', 'window.secure'],
            screw_ie8: true,
            keep_fnames: false
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
    }),

    errorLogger(error) {
        console.log(chalk.white.bgRed.bold('**** Start of Error ****'));
        console.log(chalk.red.bold(error));
        console.log(chalk.white.bgRed.bold('**** End of Error ****'));
        this.emit('end');
    },

    clean(path, done) {
        this.log('CLEANING :: ' + path);
        //Promisses com bug, se executar 2 vezes em seguida dá pau.
        //del(path).then(done());
        del.sync(path);
        done();
    },

    log(msg) {
        if (typeof (msg) === 'object') {
            for (var item in msg) {
                if (msg.hasOwnProperty(item)) {
                    $.util.log(chalk.white.bgGreen.bold(msg[item]));
                }
            }
        } else {
            $.util.log(chalk.white.bgBlue.bold(msg));
        }
    },

    verbose() {
        return $.if(args.verbose, $.print(function (filePath) {
            var prefix = chalk.red(prefix);
            var fpath = chalk.yellow(chalk.stripColor(filePath));
            $.util.log(`${prefix} ${fpath}`);
        }));
    },

    logBrowserify(file, id) {
        if (args.verbose) {
            if (id === file) {
                id = '--';
            }
            var a = chalk.red('ID:');
            var b = chalk.yellow(chalk.stripColor(id));
            var c = chalk.gray(chalk.stripColor('└─ ' + file));
            $.util.log(`${a} ${b}\r\n               ${c}`);
        }
    },

    parseRazorHTML() {
        var outstream = through().on('pipe', function (source) {
            source.unpipe(this);
            this.transformStream = source
                .pipe($.injstr.replace('<!(?:--|--\\s)helper:([^~]+?)(?:\\s--|--)>[^~]*?<!(?:--|--\\s)endhelper(?:--|\\s--)>', '$1'))
                .pipe($.rename((path) => path.basename = 'index-razor'));
        });

        outstream.pipe = function (destination, options) {
            return this.transformStream.pipe(destination, options);
        };

        return outstream;
    },

    parseRazorJsPre() {
        var outstream = through().on('pipe', function (source) {
            source.unpipe(this);
            this.transformStream = source
                .pipe($.injstr.replace('\\/(?:\\/|\\/\\s)razorsafe:([^~]*?=)([^~]*?;)[^*]+?\\/(?:\\/|\\s)endrazorsafe', '$1 "______________$2______________";'))
                .pipe($.injstr.replace('\\/(?:\\/|\\/\\s)razor:([^~]*?=)([^~]*?;)[^*]+?\\/(?:\\/|\\s)endrazor', '$1 $2'));
        });

        outstream.pipe = function (destination, options) {
            return this.transformStream.pipe(destination, options);
        };

        return outstream;
    },

    parseRazorJsPos() {
        var outstream = through().on('pipe', function (source) {
            source.unpipe(this);
            this.transformStream = source
                .pipe($.injstr.replace('______________"[;,]', ''))
                .pipe($.injstr.replace('"______________ ', ''));
        });

        outstream.pipe = function (destination, options) {
            return this.transformStream.pipe(destination, options);
        };

        return outstream;
    },

    build() {
        var rename = $.rename(function (path) {
            path.basename = 'razor';
            path.basename += args.htmlmin ? '.min' : '';
            path.extname = '.cshtml';
        });

        var htmlmin = $.htmlmin({
            //DOC: https://github.com/kangax/html-minifier
            collapseWhitespace: true,
            removeComments: true
        });

        var outstream = through().on('pipe', function (source) {
            source.unpipe(this);
            var transf = source;

            if (args.inline) {
                transf = transf
                    .pipe($.usemin({
                        html: [],
                        inlinejs: [],
                        inlinecss: []
                    }));
            }

            if (args.htmlmin) {
                transf = transf
                    .pipe($.usemin({
                        html: [htmlmin]
                    }));
            }

            transf = transf
                .pipe($.injstr.prepend(fs.readFileSync('./src/templ-razor-header.cs', 'utf-8') + '\r\n\r\n'))
                .pipe($.injstr.append('\r\n\r\n' + fs.readFileSync('./src/templ-razor-footer.cs', 'utf-8')))
                .pipe($.injstr.replace('@media', '@@media')); //escape Razor para CSS

            this.transformStream = transf.pipe(rename);
        });

        outstream.pipe = function (destination, options) {
            return this.transformStream.pipe(destination, options);
        };

        return outstream;
    }

};
