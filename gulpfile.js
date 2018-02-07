const gulp = require('gulp');
const sass = require('gulp-sass');
const typescript = require('gulp-typescript');
const sourcemaps = require('gulp-sourcemaps');
const cleanCSS = require('gulp-clean-css');
const uglify = require('gulp-uglify');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const filter = require('gulp-filter');
const util = require('gulp-util');

const compileCss = () => {
    const scssFilter = filter('**/*.scss', { restore: true });

    return gulp.src(['./css/vendor/*.css', './css/*.scss'])
               .pipe(sourcemaps.init())
               .pipe(scssFilter)
               .pipe(sass())
               .on('error', util.log)
               .pipe(scssFilter.restore)
               .pipe(cleanCSS())
               .pipe(concat('bundle.css'))
               .pipe(sourcemaps.write('.'))
               .pipe(gulp.dest('./public/css'));
}

const tsProject = typescript.createProject('tsconfig.json');

const compileJs = () => {
    return gulp.src(['./js/*.ts'])
               .pipe(sourcemaps.init())
               .pipe(tsProject())
               .on('error', util.log)
               .pipe(uglify())
               .pipe(concat('bundle.js'))
               .pipe(sourcemaps.write('.'))
               .pipe(gulp.dest('./public/js'));
}

gulp.task('compile-css', compileCss);
gulp.task('compile-js', compileJs);

gulp.task('watch-css', () => gulp.watch('./css/*.scss', ['compile-css']));
gulp.task('watch-js', () => gulp.watch('./js/*.ts', ['compile-js']));
