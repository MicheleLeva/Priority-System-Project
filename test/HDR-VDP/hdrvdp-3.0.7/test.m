function res = test ()

pat = digitsPattern;
str = join(extract("screen_29.508.png", pat), '.');
disp(str2double(str));