function X = deltaE2000_test ()

root = "test_move";
dirs = [root+"/Test-1-SP", root+"/Test-AOI"];
%dirs = [root+"/Test-1-SP"];
%dirs = [root+"/Test-AOI"];
fullDir = root+"/FullMoving";

img1 = double(imread("A.png"));
img2 = double(imread("B.png"));
diff = imcolordiff(img1, img2, "Standard", "CIEDE2000",'kL',2,'K1',0.048,'K2',0.014);
imagesc(diff);
clim([0,100]);
colorbar;
disp(mean((mean(diff))));
