/// <binding AfterBuild='minify' />
var gulp = require('gulp');
var uglify = require("gulp-uglify");
var ngAnnotate = require("gulp-ng-annotate");

// minify is the name of the task we are going to execute.
gulp.task('minify', function () {
    return gulp.src("wwwroot/js/*.js")                 // the source we want to process. all JavaScript in our JS folder.
                .pipe(ngAnnotate())
                .pipe(uglify())                         // fluent syntax. Simply take each file and minify, uglify, compress them down.
                .pipe(gulp.dest("wwwroot/lib/_app"));    // save them after they've been compressed.
});