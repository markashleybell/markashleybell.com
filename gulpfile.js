const gulp = require('gulp');
const sass = require('gulp-sass');
const typescript = require('gulp-typescript');
const cleanCSS = require('gulp-clean-css');
const uglify = require('gulp-uglify');
const concat = require('gulp-concat');
const rename = require('gulp-rename');

const tsProject = typescript.createProject('tsconfig.json');

const sources = {
    scss: './css/*.scss',
    css: './css/*.css',
    ts: './js/*.ts',
    js: './js/*.js'
};

const output = {
    css: './public/css',
    js: './public/js'
};

const scss = gulp.src(sources.scss);
const css = gulp.src(sources.css);
const ts = gulp.src(sources.ts);
const js = gulp.src(sources.js);

const toSameFolder = gulp.dest(filename => filename.base);

const compileScss = () => scss.pipe(sass()).pipe(toSameFolder);
const compileTs = () => ts.pipe(tsProject()).pipe(toSameFolder);

const packCss = () => css.pipe(cleanCSS()).pipe(concat('bundle.css')).pipe(gulp.dest(output.css));
const packJs = () => js.pipe(uglify()).pipe(concat('bundle.js')).pipe(gulp.dest(output.js));

gulp.task('compile-scss', compileScss);
gulp.task('compile-typescript', compileTs);

gulp.task('pack-css', ['compile-scss'], packCss);
gulp.task('pack-js', ['compile-typescript'], packJs);

gulp.task('watch-typescript', () => gulp.watch(sources.ts, ['pack-js']));
gulp.task('watch-scss', () => gulp.watch(sources.scss, ['pack-css']));
