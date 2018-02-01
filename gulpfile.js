const gulp = require('gulp');
const sass = require('gulp-sass');
const typescript = require('gulp-typescript');
const sourcemaps = require('gulp-sourcemaps');
const cleanCSS = require('gulp-clean-css');
const uglify = require('gulp-uglify');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const filter = require('gulp-filter');

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

const scssFilter = filter('**/*.scss', { restore: true });

const compileCss = () => {
    gulp.src(['./css/vendor/*.css', './css/*.scss'])
    .pipe(sourcemaps.init())
    .pipe(scssFilter)
    .pipe(sass())
    .pipe(scssFilter.restore)
    .pipe(cleanCSS())
    .pipe(concat('bundle.css'))
    .pipe(sourcemaps.write('.'))
    .pipe(gulp.dest(output.css));
}

// gulp.task('compile-js', compileJs);
gulp.task('compile-css', compileCss);

// gulp.task('watch-js', () => gulp.watch(sources.ts, ['pack-js']));
// gulp.task('watch-css', () => gulp.watch(sources.scss, ['pack-css']));
