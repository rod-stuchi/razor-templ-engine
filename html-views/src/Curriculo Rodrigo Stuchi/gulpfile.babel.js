import gulpLoadPlugins from 'gulp-load-plugins';
import browser_sync from 'browser-sync';
import yargs from 'yargs';
import clear from 'clear';
import gulp from 'gulp';
import f from './gulpfunc';

const $ = gulpLoadPlugins({ lazy: true });
const browserSync = browser_sync.create();
const args = yargs.argv;

//gulp default task
const gulpDefault = gulp.series(clean, copy, gulp.parallel(style, script), inject, live);
gulpDefault.description = 'gulp task padrão: use apenas gulp';
export default gulpDefault;

export { clean, copy, style, script, build };

// -----------------------------------------------------------------------------------------------------------------
//                                               INFRA
// -----------------------------------------------------------------------------------------------------------------

function clean(done) {
    f.log('CLEANING ALL IN FILES');

    f.clean(['./_dist/*.*', './_dist/**/*.*', './_dist/imgs'], done);
}
clean.description = 'limpa o diretório [_dist]';

function copy() {
    f.log('COPYING ASSETS');

    return gulp.src('./src/imgs/*.*')
        .pipe(f.verbose('FROM: '))
        .pipe(gulp.dest('./_dist/imgs'))
        .pipe(f.verbose('TO: '));
}
copy.description = 'copia imagens para o diretório [_dist]';

function htmlWatch() {
    f.log('WATCHING HTML :: INJECT');

    gulp.watch(['./src/*.html'], inject);
}

function inject() {
    f.log('INJECTING RESOURCES IN HTML');

    var svgs = gulp
        .src('./src/imgs/*.svg')
        .pipe($.svgstore({ inlineSvg: true }));

    return gulp.src('./src/index.html')
        .pipe(f.verbose())
        .pipe($.inject(svgs, { transform: (path, file) =>  file.contents.toString() }))
        .pipe($.inject(
            gulp.src(['./_dist/*.js', './_dist/*.css'], {
                read: false
            }), {
                relative: false,
                ignorePath: ['/_dist']
            }
        ))
        .pipe(gulp.dest('./_dist'));
}

function injectRazor(done) {
    gulp.series(
        scriptRazor,
        () => {
            f.log('INJECTING RESOURCES IN HTML :: RAZOR');

            var svgs = gulp
                .src('./src/imgs/*.svg')
                .pipe($.svgstore({ inlineSvg: true }));

            return gulp.src('./src/index.html')
                .pipe(f.verbose())
                .pipe($.inject(svgs, { transform: (path, file) =>  file.contents.toString() }))
                .pipe($.inject(
                    gulp.src(['./_dist/*razor*.js', './_dist/*.css'], {
                        read: false
                    }), {
                        relative: false,
                        ignorePath: ['/_dist']
                    }
                ))
                .pipe(f.parseRazorHTML())
                .pipe(gulp.dest('./_dist'));
        }
    )(done);
}

function live(done) {

    gulp.parallel(
        styleWatch,
        scriptWatch,
        htmlWatch
    )(done);

    browserSync.init({
        server: {
            baseDir: './_dist/',
            index: 'index.html'
        }
    });

    gulp.watch(['./_dist/*.html', './_dist/*.js']).on('change', browserSync.reload);
}



// -----------------------------------------------------------------------------------------------------------------
//                                               SCRIPTS
// -----------------------------------------------------------------------------------------------------------------
function script(done) {
    if (args.check) {
        gulp.series(scriptCheck, scriptWatchCheck)(done);
    } else {
        gulp.series(
            scriptClean,
            () => {
                f.log('BUILDING SCRIPTS');

                return gulp.src(['./src/*.js'])
                    .pipe($.concat('bundle.js'))
                    .pipe(gulp.dest('./_dist/'));
            }
        )(done);
    }

}
script.description = 'compila Javascript / Typescript + Browserify para blundle.js';
script.flags = {
    '--check': 'analisa Javascript / Typescript por inconsistências'
};

function scriptCheck() {
    clear(); // limpa a tela do terminal / console.
    f.log('CHECKING SCRIPTS :: ESLINT');

    return gulp.src(['./*.js', './src/*.js'])
        .pipe(f.verbose())
        .pipe($.eslint())
        .pipe($.eslint.format());
        //.pipe($.eslint.failAfterError());
}

function scriptClean(done) {
    f.log('CLEANING :: JS');

    f.clean(['./_dist/*.js', './_dist/**/*.js'], done);
}

function scriptRazor(done) {
    gulp.series(
        script,
        () => {
            f.log('BUILDING SCRIPTS :: RAZOR');

            return gulp.src(['./_dist/*.js'])
                .pipe(f.parseRazorJsPre())
                .pipe(f.UglifyConfig)
                .pipe(f.parseRazorJsPos())
                .pipe($.rename((path) => path.basename = 'blundle-razor.min'))
                .pipe(gulp.dest('./_dist'));
        })(done);
}

function scriptWatch() {
    f.log('WATCHING SCRIPTS :: BUILD');

    gulp.watch(['./src/*.js', './src/*.ts'], script);
}

function scriptWatchCheck() {
    f.log('WATCHING SCRIPTS :: CHECK');

    gulp.watch(['./*.js', './src/*.js'], scriptCheck);
}


// // -----------------------------------------------------------------------------------------------------------------
// //                                               STYLES
// // -----------------------------------------------------------------------------------------------------------------
function style(done) {
    if (args.check) {
        gulp.series(styleCheck, styleWatchCheck)(done);
    }
    else {
        gulp.series(
            styleClean,
            () => {
                f.log('BUILDING STYLES');

                return gulp.src('./src/*.scss')
                    .pipe(f.verbose('SOURCE: '))
                    .pipe($.sass().on('error', f.errorLogger))
                    .pipe($.autoprefixer({
                        browsers: ['last 2 versions', '> 5%']
                    }))
                    .pipe($.concatCss('bundle.css'))
                    .pipe($.shorthand())
                    .pipe($.uglifycss())
                    .pipe($.replace(/"(?:..\/)+/gmi, '"/')) // replace ../ to /
                    .pipe($.base64({
                        debug: false,
                        extensions: ['jpg', 'png', 'gif']
                    }))
                    .pipe(gulp.dest('./_dist'))
                    .pipe(f.verbose('BUILD: '))
                    .pipe(browserSync.stream());
            }
        )(done);
    }

}
style.description = 'compila SCSS para CSS';
style.flags = {
    '--check': 'analisa SCSS por inconsistências'
};

function styleCheck() {
    clear(); // limpa a tela do terminal / console.
    f.log('CHECKING STYLES :: SASSLINT');

    return gulp.src('./src/*.scss')
        .pipe(f.verbose())
        .pipe($.sassLint({
            options: {
                formatter: 'stylish',
                'merge-default-rules': true
            },
            rules: {
                'no-important': 0
            }
        }))
        .pipe($.sassLint.format())
        .pipe($.sassLint.failOnError());
}

function styleClean(done) {
    f.log('CLEANING :: CSS');

    f.clean(['./_dist/*.css', './_dist/**/*.css'], done);
}

function styleWatch() {
    f.log('WATCHING STYLES :: BUILD');

    gulp.watch(['./src/*.scss'], style);
}

function styleWatchCheck() {
    f.log('WATCHING STYLES :: CHECK');

    gulp.watch(['./src/*.scss'], styleCheck);
}


// -----------------------------------------------------------------------------------------------------------------
//                                               BUILD ALL MIN
// -----------------------------------------------------------------------------------------------------------------
function build(done) {
    gulp.series(
        clean,
        copy,
        style,
        injectRazor,
        () => {
            f.log('ASSEMBLING HTML.');

            return gulp.src('./_dist/index-razor.html')
                .pipe(f.verbose())
                .pipe(f.build())
                .pipe(gulp.dest('./_dist/'));
        }
    )(done);
}

build.description = 'Compila HTML para CSHTML (razor)';
build.flags = {
    '--htmlmin': 'compacta o arquivo CSHTML',
    '--inline': 'injeta conteúdo dos arquivos .JS e .CSS no arquivo CSHTML',
};