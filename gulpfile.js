var gulp = require('gulp'),
    sass = require('gulp-sass'),
    typescript = require('gulp-typescript'),
    cleanCSS = require('gulp-clean-css'),
    uglify = require('gulp-uglify'),
    concat = require('gulp-concat'),
    rename = require('gulp-rename');

var sources = {
    scss: './css/*.scss',
    css: './css/*.css',
    ts: './js/*.ts',
    js: './js/*.js'
};

var output = {
    css: './public/css',
    js: './public/js'
};

gulp.task('compile-sass', function() {
    return gulp.src(sources.scss)
               .pipe(sass())
               .pipe(gulp.dest(function(f) {
                   return f.base;
               }));
});

gulp.task('compile-typescript', function () {
    return gulp.src(sources.ts)
               .pipe(typescript())
               .pipe(gulp.dest(function(f) {
                   return f.base;
               }));
});

gulp.task('pack-js', ['compile-typescript'], function () {
    return gulp.src(sources.js)
               .pipe(uglify())
               .pipe(concat('bundle.js'))
               .pipe(gulp.dest(output.js));
});

gulp.task('pack-css', ['compile-sass'], function () {
    return gulp.src(sources.css)
               .pipe(cleanCSS())
               .pipe(concat('bundle.css'))
               .pipe(gulp.dest(output.css));
});

gulp.task('watch-typescript', function() {
    gulp.watch(sources.ts, ['pack-js']);
});

gulp.task('watch-sass', function() {
    gulp.watch(sources.scss, ['pack-css']);
});